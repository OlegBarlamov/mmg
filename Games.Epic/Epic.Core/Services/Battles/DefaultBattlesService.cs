using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Logic;
using Epic.Core.Services.BattleDefinitions;
using Epic.Core.Services.Players;
using Epic.Core.Services.Units;
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
        [NotNull] private IPlayerUnitsService PlayerUnitsService { get; }
        [NotNull] private IPlayersService PlayersService { get; }

        public DefaultBattlesService(
            [NotNull] IBattlesRepository battlesRepository,
            [NotNull] IBattleUnitsService battleUnitsService,
            [NotNull] IBattleDefinitionsService battleDefinitionsService,
            [NotNull] IBattlesCacheService battlesCacheService,
            [NotNull] IPlayerUnitsService playerUnitsService,
            [NotNull] IPlayersService playersService)
        {
            BattlesRepository = battlesRepository ?? throw new ArgumentNullException(nameof(battlesRepository));
            BattleUnitsService = battleUnitsService ?? throw new ArgumentNullException(nameof(battleUnitsService));
            BattleDefinitionsService = battleDefinitionsService ?? throw new ArgumentNullException(nameof(battleDefinitionsService));
            BattlesCacheService = battlesCacheService ?? throw new ArgumentNullException(nameof(battlesCacheService));
            PlayerUnitsService = playerUnitsService ?? throw new ArgumentNullException(nameof(playerUnitsService));
            PlayersService = playersService ?? throw new ArgumentNullException(nameof(playersService));
        }

        public async Task<IBattleObject> GetBattleById(Guid battleId)
        {
            var uploadedBattle = BattlesCacheService.FindBattleById(battleId);
            if (uploadedBattle != null)
                return uploadedBattle;
            
            var battleEntity = await BattlesRepository.GetBattleByIdAsync(battleId);
            var battleObject = MutableBattleObject.FromEntity(battleEntity);
            await FillUnits(battleObject);
            await FillUsers(battleObject);

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
            await FillUsers(battleObject);
            
            return battleObject;
        }

        public async Task<IBattleObject> CreateBattleFromDefinition(Guid playerId, Guid battleDefinitionId)
        {
            var player = await PlayersService.GetById(playerId);
            if (player.IsDefeated)
                throw new InvalidOperationException("Player is already defeated.");
            
            var battleDefinition =
                await BattleDefinitionsService.GetBattleDefinitionByPlayerAndId(playerId, battleDefinitionId);
            
            if (battleDefinition.IsFinished)
                throw new InvalidOperationException("Battle definition is finished");
            
            var battleEntity = await BattlesRepository.CreateBattleAsync(
                battleDefinitionId,
                new[] { playerId },
                battleDefinition.Width,
                battleDefinition.Height,
                false);
            
            var battleObject = MutableBattleObject.FromEntity(battleEntity);
            
            var battleInitialUnits = await BattleUnitsService.CreateUnitsFromBattleDefinition(battleDefinition, battleObject.Id);
            battleObject.Units = new List<MutableBattleUnitObject>(battleInitialUnits.Select(MutableBattleUnitObject.CopyFrom));
            
            await FillUsers(battleObject);
            
            return battleObject;
        }

        public async Task<IBattleObject> BeginBattle(Guid playerId, IBattleObject battleObject)
        {
            var player = await PlayersService.GetById(playerId);
            if (player.IsDefeated)
                throw new InvalidOperationException("Player is already defeated.");
            
            var mutableBattleObject = ToMutableObject(battleObject);
            mutableBattleObject.IsActive = true;
            await BattlesRepository.UpdateBattle(MutableBattleObject.ToEntity(mutableBattleObject));
            
            var userUnits = await PlayerUnitsService.GetAliveUnitsByContainerId(player.ArmyContainerId);
            var userBattleUnits = await BattleUnitsService.CreateUnitsFromPlayerUnits(userUnits, InBattlePlayerNumber.Player1, battleObject.Id);
            mutableBattleObject.Units.AddRange(userBattleUnits.Select(MutableBattleUnitObject.CopyFrom));

            PlaceBattleUnits(mutableBattleObject);

            await BattleUnitsService.UpdateUnits(mutableBattleObject.Units);
            
            return mutableBattleObject;
        }

        public Task UpdateBattle(IBattleObject battleObject)
        {
            return BattlesRepository.UpdateBattle(MutableBattleObject.ToEntity(battleObject));
        }

        public async Task FinishBattle(IBattleObject battleObject, BattleResult result)
        {
            var mutableObject = ToMutableObject(battleObject);
            mutableObject.IsActive = false;
            await UpdateBattle(mutableObject);

            await BattleDefinitionsService.SetFinished(battleObject.BattleDefinitionId);
        }

        private void PlaceBattleUnits(MutableBattleObject battleObject)
        {
            var unitsPlayer1Index = 0;
            var unitsPlayer2Index = 0;
            battleObject.Units.ForEach(u =>
            {
                if (u.PlayerIndex == (int)InBattlePlayerNumber.Player1)
                {
                    u.Column = 0;
                    u.Row = unitsPlayer1Index++;
                }
                else if (u.PlayerIndex == (int)InBattlePlayerNumber.Player2)
                {
                    u.Column = battleObject.Width - 1;
                    u.Row = unitsPlayer2Index++;
                }
                else
                {
                    u.Column = -1;
                    u.Row = -1;
                }
            });
        }

        private async Task FillUnits(MutableBattleObject battleObject)
        {
            var battleUnits = await BattleUnitsService.GetBattleUnits(battleObject.Id);
            var mutableBattleUnits = battleUnits.Select(MutableBattleUnitObject.CopyFrom);
            battleObject.Units = new List<MutableBattleUnitObject>(mutableBattleUnits);
        }

        private async Task FillUsers(MutableBattleObject battleObject)
        {
            var userIds = await BattlesRepository.GetBattleUsers(battleObject.Id);
            battleObject.PlayerIds = new List<Guid>(userIds);
        }

        private MutableBattleObject ToMutableObject(IBattleObject battleObject)
        {
            return (MutableBattleObject)battleObject;
        }
    }
}