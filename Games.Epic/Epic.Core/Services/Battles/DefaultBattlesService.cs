using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Logic;
using Epic.Core.Services.BattleDefinitions;
using Epic.Core.Services.BattleObstacles;
using Epic.Core.Services.Players;
using Epic.Core.Services.Units;
using Epic.Core.Services.UnitsContainers;
using Epic.Core.Services.UnitTypes;
using Epic.Data.BattleReports;
using Epic.Data.Battles;
using JetBrains.Annotations;

namespace Epic.Core.Services.Battles
{
    [UsedImplicitly]
    public class DefaultBattlesService : IBattlesService
    {
        [NotNull] private IBattlesRepository BattlesRepository { get; }
        [NotNull] private IBattleUnitsService BattleUnitsService { get; }
        [NotNull] private IBattleDefinitionsService BattleDefinitionsService { get; }
        [NotNull] private IBattlesCacheService BattlesCacheService { get; }
        [NotNull] private IGlobalUnitsService GlobalUnitsService { get; }
        [NotNull] private IPlayersService PlayersService { get; }
        [NotNull] private IBattleReportsRepository BattleReportsRepository { get; }
        [NotNull] private IBattleUnitsPlacer BattleUnitsPlacer { get; }
        [NotNull] private IUnitsContainersService UnitsContainersService { get; }
        public IUnitTypesService UnitTypesService { get; }
        public IBattleObstaclesService BattleObstaclesService { get; }
        public IBattleObstaclesGenerator BattleObstaclesGenerator { get; }

        public DefaultBattlesService(
            [NotNull] IBattlesRepository battlesRepository,
            [NotNull] IBattleUnitsService battleUnitsService,
            [NotNull] IBattleDefinitionsService battleDefinitionsService,
            [NotNull] IBattlesCacheService battlesCacheService,
            [NotNull] IGlobalUnitsService globalUnitsService,
            [NotNull] IPlayersService playersService,
            [NotNull] IBattleReportsRepository battleReportsRepository,
            [NotNull] IBattleUnitsPlacer battleUnitsPlacer,
            [NotNull] IUnitsContainersService unitsContainersService,
            [NotNull] IUnitTypesService unitTypesService,
            [NotNull] IBattleObstaclesService battleObstaclesService,
            [NotNull] IBattleObstaclesGenerator battleObstaclesGenerator)
        {
            BattlesRepository = battlesRepository ?? throw new ArgumentNullException(nameof(battlesRepository));
            BattleUnitsService = battleUnitsService ?? throw new ArgumentNullException(nameof(battleUnitsService));
            BattleDefinitionsService = battleDefinitionsService ?? throw new ArgumentNullException(nameof(battleDefinitionsService));
            BattlesCacheService = battlesCacheService ?? throw new ArgumentNullException(nameof(battlesCacheService));
            GlobalUnitsService = globalUnitsService ?? throw new ArgumentNullException(nameof(globalUnitsService));
            PlayersService = playersService ?? throw new ArgumentNullException(nameof(playersService));
            BattleReportsRepository = battleReportsRepository ?? throw new ArgumentNullException(nameof(battleReportsRepository));
            BattleUnitsPlacer = battleUnitsPlacer ?? throw new ArgumentNullException(nameof(battleUnitsPlacer));
            UnitsContainersService = unitsContainersService ?? throw new ArgumentNullException(nameof(unitsContainersService));
            UnitTypesService = unitTypesService ?? throw new ArgumentNullException(nameof(unitTypesService));
            BattleObstaclesService = battleObstaclesService ?? throw new ArgumentNullException(nameof(battleObstaclesService));
            BattleObstaclesGenerator = battleObstaclesGenerator ?? throw new ArgumentNullException(nameof(battleObstaclesGenerator));
        }

        public async Task<IBattleObject> GetBattleById(Guid battleId)
        {
            var uploadedBattle = BattlesCacheService.FindBattleById(battleId);
            if (uploadedBattle != null)
                return uploadedBattle;
            
            var battleEntity = await BattlesRepository.GetBattleByIdAsync(battleId);
            var battleObject = MutableBattleObject.FromEntity(battleEntity);
            await FillUnits(battleObject);
            await FillPlayers(battleObject);
            await FillObstacles(battleObject);

            return battleObject;
        }

        public async Task<IBattleObject> FindActiveBattleByPlayerId(Guid playerId)
        {
            var battleEntity = await BattlesRepository.FindActiveBattleByPlayerIdAsync(playerId);
            if (battleEntity == null)
                return null;

            var uploadedBattle = BattlesCacheService.FindBattleById(battleEntity.Id);
            if (uploadedBattle != null)
                return uploadedBattle;
            
            var battleObject = MutableBattleObject.FromEntity(battleEntity);
            await FillUnits(battleObject);
            await FillPlayers(battleObject);
            await FillObstacles(battleObject);
            
            return battleObject;
        }

        public async Task<IBattleObject> CreateBattleFromDefinition(IPlayerObject playerObject, Guid battleDefinitionId)
        {
            var battleDefinition =
                await BattleDefinitionsService.GetBattleDefinitionByPlayerAndId(playerObject.Id, battleDefinitionId);
            return await CreateBattleFromDefinition(playerObject, battleDefinition, true);
        }

        public async Task<IBattleObject> CreateBattleFromDefinition(Guid playerId, IBattleDefinitionObject battleDefinitionObject, bool progressDays)
        {
            var player = await PlayersService.GetById(playerId);
            return await CreateBattleFromDefinition(player, battleDefinitionObject, progressDays);
        }

        public async Task<IBattleObject> CreateBattleFromDefinition(IPlayerObject playerObject, IBattleDefinitionObject battleDefinitionObject, bool progressDays)
        {
            if (battleDefinitionObject.IsFinished)
                throw new InvalidOperationException("Battle definition is finished");
            if (battleDefinitionObject.ExpireAtDay <= playerObject.Day)
                throw new InvalidOperationException("Battle definition is expired");
            
            var battleEntity = await BattlesRepository.CreateBattleAsync(
                battleDefinitionObject.Id,
                new[] { playerObject.Id },
                battleDefinitionObject.Width,
                battleDefinitionObject.Height,
                false,
                progressDays
                );
            
            var battleObject = MutableBattleObject.FromEntity(battleEntity);
            
            var obstacles = await BattleObstaclesGenerator.GenerateForBattle(battleObject);
            battleObject.Obstacles = obstacles;
            
            var battleInitialUnits = await BattleUnitsService.CreateUnitsFromBattleDefinition(battleDefinitionObject, battleObject.Id);
            battleObject.Units = new List<MutableBattleUnitObject>(battleInitialUnits.Select(MutableBattleUnitObject.CopyFrom));
            
            await FillPlayers(battleObject);
            
            return battleObject;
        }

        public async Task<IBattleObject> CreateBattleFromPlayerEnemy(IPlayerObject player, IPlayerObject enemyPlayer)
        {
            var stage = player.Stage;
            var battleDef = await BattleDefinitionsService.CreateBattleDefinition(14, 7, stage);
            var battleEntity = await BattlesRepository.CreateBattleAsync(
                battleDef.Id,
                new[] { player.Id, enemyPlayer.Id },
                battleDef.Width,
                battleDef.Height,
                false,
                false);
            
            var battleObject = MutableBattleObject.FromEntity(battleEntity);
            var enemyGlobalUnits = await GlobalUnitsService.GetAliveUnitsByContainerId(enemyPlayer.ActiveHero.ArmyContainerId);
            var battleInitialUnits = await BattleUnitsService.CreateBattleUnitsFromGlobalUnits(
                enemyGlobalUnits, 
                InBattlePlayerNumber.Player2, 
                battleObject.Id,
                enemyPlayer.ActiveHero);
            
            var obstacles = await BattleObstaclesGenerator.GenerateForBattle(battleObject);
            battleObject.Obstacles = obstacles;
            
            battleObject.Units = new List<MutableBattleUnitObject>(battleInitialUnits.Select(MutableBattleUnitObject.CopyFrom));
            
            await FillPlayers(battleObject);
            
            return battleObject;
        }

        public async Task<IBattleObject> BeginBattle(Guid playerId, IBattleObject battleObject)
        {
            var player = await PlayersService.GetById(playerId);
            var mutableBattleObject = ToMutableObject(battleObject);
            mutableBattleObject.IsActive = true;
            await BattlesRepository.UpdateBattle(MutableBattleObject.ToEntity(mutableBattleObject));
            
            var userUnits = await GlobalUnitsService.GetAliveUnitsByContainerId(player.ActiveHero.ArmyContainerId);
            var userUnitsFitToBattle = BattleUnitsService.PickUnitsFitToBattleSize(userUnits, mutableBattleObject);
            var userBattleUnits = await BattleUnitsService.CreateBattleUnitsFromGlobalUnits(
                userUnitsFitToBattle, 
                InBattlePlayerNumber.Player1, 
                battleObject.Id,
                player.ActiveHero);
            
            mutableBattleObject.Units.AddRange(userBattleUnits.Select(MutableBattleUnitObject.CopyFrom));

            int? player1OriginalContainerHeight = player.ActiveHero?.ArmyContainer?.Capacity;
            int? player2OriginalContainerHeight = null;
            var player2ContainerId = mutableBattleObject.Units
                .FirstOrDefault(u => u.PlayerIndex == (int)InBattlePlayerNumber.Player2 && u.GlobalUnit != null)
                ?.GlobalUnit
                ?.ContainerId;
            if (player2ContainerId.HasValue)
                player2OriginalContainerHeight = (await UnitsContainersService.GetById(player2ContainerId.Value)).Capacity;

            BattleUnitsPlacer.PlaceBattleUnitsDefaultPattern(
                mutableBattleObject,
                player1OriginalContainerHeight,
                player2OriginalContainerHeight);

            await BattleUnitsService.UpdateUnits(mutableBattleObject.Units);
            
            return mutableBattleObject;
        }

        public Task UpdateBattle(IBattleObject battleObject)
        {
            return BattlesRepository.UpdateBattle(MutableBattleObject.ToEntity(battleObject));
        }

        public async Task<IBattleReportEntity> FinishBattle(IBattleObject battleObject, BattleResult result)
        {
            var mutableObject = ToMutableObject(battleObject);
            mutableObject.IsActive = false;
            await UpdateBattle(mutableObject);

            await BattleDefinitionsService.SetFinished(battleObject.BattleDefinitionId);

            return await BattleReportsRepository.Create(new MutableBattleReportFields(
                battleObject.Id, 
                result.Winner.HasValue ? (int?)result.Winner.Value : null,
                battleObject.PlayerInfos.Select(x => x.PlayerId).ToArray(),
                result.Winner.HasValue ? battleObject.FindPlayerId(result.Winner.Value) : null)
            );
        }

        public Task<int> CalculateRansomValueForPlayer(Guid playerId, IBattleObject battleObject)
        {
            var playerInfo = battleObject.PlayerInfos.FirstOrDefault(x  => x.PlayerId == playerId);
            if (playerInfo == null)
                throw new InvalidOperationException($"Player {playerId} is not present in the battle {battleObject.Id}");

            var battlePlayerNumber = battleObject.FindPlayerNumber(playerInfo);
            if (!battlePlayerNumber.HasValue)
                throw new InvalidOperationException($"Player {playerId} is not present in the battle {battleObject.Id}");
                
            var playerAliveUnits = battleObject.Units
                .Where(x => x.PlayerIndex == (int)battlePlayerNumber && x.GlobalUnit.IsAlive)
                .ToArray();
            
            return Task.FromResult(playerAliveUnits
                .Sum(x => x.GlobalUnit.UnitType.Value * x.GlobalUnit.Count));
        }

        public Task UpdateInBattlePlayerInfo(IPlayerInBattleInfoObject playerInBattleInfoObject)
        {
            var entity = playerInBattleInfoObject.ToEntity();
            return BattlesRepository.UpdatePlayerToBattle(entity);
        }

        private async Task FillUnits(MutableBattleObject battleObject)
        {
            var battleUnits = await BattleUnitsService.GetBattleUnits(battleObject.Id);
            var mutableBattleUnits = battleUnits.Select(MutableBattleUnitObject.CopyFrom);
            battleObject.Units = new List<MutableBattleUnitObject>(mutableBattleUnits);
        }

        private async Task FillObstacles(MutableBattleObject battleObject)
        {
            var obstacles = await BattleObstaclesService.GetForBattle(battleObject.Id);
            battleObject.Obstacles = obstacles;
        }

        private async Task FillPlayers(MutableBattleObject battleObject)
        {
            var playerToBattles = await BattlesRepository.GetBattlePlayers(battleObject.Id);
            battleObject.PlayerInfos = playerToBattles.Select(MutablePlayerInBattleInfoObject.FromEntity).ToList();
        }

        private MutableBattleObject ToMutableObject(IBattleObject battleObject)
        {
            return (MutableBattleObject)battleObject;
        }
    }
}