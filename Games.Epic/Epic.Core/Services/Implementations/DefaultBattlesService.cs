using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Objects.Battle;
using Epic.Core.Objects.BattleUnit;
using Epic.Data.Battles;
using JetBrains.Annotations;

namespace Epic.Core
{
    [UsedImplicitly]
    public class DefaultBattlesService : IBattlesService
    {
        [NotNull] public IBattlesRepository BattlesRepository { get; }
        [NotNull] public IBattleUnitsService BattleUnitsService { get; }
        [NotNull] public IBattleDefinitionsService BattleDefinitionsService { get; }
        [NotNull] public IBattlesCacheService BattlesCacheService { get; }
        public IUserUnitsService UserUnitsService { get; }

        public DefaultBattlesService(
            [NotNull] IBattlesRepository battlesRepository,
            [NotNull] IBattleUnitsService battleUnitsService,
            [NotNull] IBattleDefinitionsService battleDefinitionsService,
            [NotNull] IBattlesCacheService battlesCacheService,
            [NotNull] IUserUnitsService userUnitsService)
        {
            BattlesRepository = battlesRepository ?? throw new ArgumentNullException(nameof(battlesRepository));
            BattleUnitsService = battleUnitsService ?? throw new ArgumentNullException(nameof(battleUnitsService));
            BattleDefinitionsService = battleDefinitionsService ?? throw new ArgumentNullException(nameof(battleDefinitionsService));
            BattlesCacheService = battlesCacheService ?? throw new ArgumentNullException(nameof(battlesCacheService));
            UserUnitsService = userUnitsService ?? throw new ArgumentNullException(nameof(userUnitsService));
        }

        public async Task<IBattleObject> GetBattleById(Guid battleId)
        {
            var uploadedBattle = BattlesCacheService.FindBattleById(battleId);
            if (uploadedBattle != null)
                return uploadedBattle;
            
            var battleEntity = await BattlesRepository.GetBattleByIdAsync(battleId);
            var battleObject = ToBattleObject(battleEntity);
            await FillUnits(battleObject);

            return battleObject;
        }

        public async Task<IBattleObject> FindActiveBattleByUserId(Guid userId)
        {
            var battleEntity = await BattlesRepository.FindActiveBattleByUserIdAsync(userId);
            if (battleEntity == null)
                return null;

            var uploadedBattle = BattlesCacheService.FindBattleById(battleEntity.Id);
            if (uploadedBattle != null)
                return uploadedBattle;
            
            var battleObject = ToBattleObject(battleEntity);
            await FillUnits(battleObject);
            
            return battleObject;
        }

        public async Task<IBattleObject> CreateBattleFromDefinition(Guid userId, Guid battleDefinitionId)
        {
            var battleDefinition =
                await BattleDefinitionsService.GetBattleDefinitionByUserAndId(userId, battleDefinitionId);
            
            var battleEntity = await BattlesRepository.CreateBattleAsync(
                battleDefinitionId,
                new[] { userId },
                battleDefinition.Width,
                battleDefinition.Height,
                false);
            
            var battleObject = ToBattleObject(battleEntity);
            
            var battleInitialUnits = await BattleUnitsService.CreateUnitsFromBattleDefinition(battleDefinition, battleObject.Id);
            battleObject.Units = new List<MutableBattleUnitObject>(battleInitialUnits.Select(MutableBattleUnitObject.CopyFrom));
            
            return battleObject;
        }

        public async Task<IBattleObject> BeginBattle(Guid userId, IBattleObject battleObject)
        {
            var mutableBattleObject = ToMutableObject(battleObject);
            mutableBattleObject.IsActive = true;
            await BattlesRepository.UpdateBattle(ToBattleEntity(mutableBattleObject));
            
            var userUnits = await UserUnitsService.GetAliveUnitsByUserAsync(userId);
            var userBattleUnits = await BattleUnitsService.CreateUnitsFromUserUnits(userUnits, InBattlePlayerNumber.Player1, battleObject.Id);
            mutableBattleObject.Units.AddRange(userBattleUnits.Select(MutableBattleUnitObject.CopyFrom));

            PlaceBattleUnits(mutableBattleObject);

            await BattleUnitsService.UpdateUnits(mutableBattleObject.Units);
            
            return mutableBattleObject;
        }

        public Task UpdateBattle(IBattleObject battleObject)
        {
            return BattlesRepository.UpdateBattle(ToBattleEntity(battleObject));
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

        private IBattleEntity ToBattleEntity(IBattleObject battleObject)
        {
            return new MutableBattleEntity
            {
                Id = battleObject.Id,
                TurnIndex = battleObject.TurnIndex,
                Width = battleObject.Width,
                Height = battleObject.Height,
                IsActive = battleObject.IsActive,
            };
        }

        private MutableBattleObject ToMutableObject(IBattleObject battleObject)
        {
            return (MutableBattleObject)battleObject;
        }

        private MutableBattleObject ToBattleObject(IBattleEntity entity)
        {
            return new MutableBattleObject
            {
                Id = entity.Id,
                TurnIndex = entity.TurnIndex,
                Width = entity.Width,
                Height = entity.Height,
                IsActive = entity.IsActive,
                Units = null,
                TurnPlayerIndex = 0,
            };
        }
    }
}