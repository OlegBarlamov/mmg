using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Logic;
using Epic.Core.Services.BattleDefinitions;
using Epic.Core.Services.Players;
using Epic.Core.Services.Units;
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

        public DefaultBattlesService(
            [NotNull] IBattlesRepository battlesRepository,
            [NotNull] IBattleUnitsService battleUnitsService,
            [NotNull] IBattleDefinitionsService battleDefinitionsService,
            [NotNull] IBattlesCacheService battlesCacheService,
            [NotNull] IGlobalUnitsService globalUnitsService,
            [NotNull] IPlayersService playersService,
            [NotNull] IBattleReportsRepository battleReportsRepository,
            [NotNull] IBattleUnitsPlacer battleUnitsPlacer)
        {
            BattlesRepository = battlesRepository ?? throw new ArgumentNullException(nameof(battlesRepository));
            BattleUnitsService = battleUnitsService ?? throw new ArgumentNullException(nameof(battleUnitsService));
            BattleDefinitionsService = battleDefinitionsService ?? throw new ArgumentNullException(nameof(battleDefinitionsService));
            BattlesCacheService = battlesCacheService ?? throw new ArgumentNullException(nameof(battlesCacheService));
            GlobalUnitsService = globalUnitsService ?? throw new ArgumentNullException(nameof(globalUnitsService));
            PlayersService = playersService ?? throw new ArgumentNullException(nameof(playersService));
            BattleReportsRepository = battleReportsRepository ?? throw new ArgumentNullException(nameof(battleReportsRepository));
            BattleUnitsPlacer = battleUnitsPlacer ?? throw new ArgumentNullException(nameof(battleUnitsPlacer));
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
            
            var battleInitialUnits = await BattleUnitsService.CreateUnitsFromBattleDefinition(battleDefinitionObject, battleObject.Id);
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

            BattleUnitsPlacer.PlaceBattleUnitsDefaultPattern(mutableBattleObject);

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
                battleObject.PlayersIds.ToArray(),
                result.Winner.HasValue ? battleObject.GetPlayerId(result.Winner.Value) : null)
            );
        }

        private async Task FillUnits(MutableBattleObject battleObject)
        {
            var battleUnits = await BattleUnitsService.GetBattleUnits(battleObject.Id);
            var mutableBattleUnits = battleUnits.Select(MutableBattleUnitObject.CopyFrom);
            battleObject.Units = new List<MutableBattleUnitObject>(mutableBattleUnits);
        }

        private async Task FillPlayers(MutableBattleObject battleObject)
        {
            var playerIds = await BattlesRepository.GetBattlePlayers(battleObject.Id);
            battleObject.PlayerIds = new List<Guid>(playerIds);
        }

        private MutableBattleObject ToMutableObject(IBattleObject battleObject)
        {
            return (MutableBattleObject)battleObject;
        }
    }
}