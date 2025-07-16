using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Logic;
using Epic.Core.Objects.Battle;
using Epic.Core.Objects.BattleUnit;
using Epic.Data.Battles;
using JetBrains.Annotations;

namespace Epic.Core
{
    [UsedImplicitly]
    public class DefaultBattlesService : IBattlesService
    {
        [NotNull] private IBattlesRepository BattlesRepository { get; }
        [NotNull] private IBattleUnitsService BattleUnitsService { get; }
        [NotNull] private IBattleDefinitionsService BattleDefinitionsService { get; }
        [NotNull] private IBattlesCacheService BattlesCacheService { get; }
        [NotNull] private IUserUnitsService UserUnitsService { get; }
        [NotNull] private IUsersService UsersService { get; }

        public DefaultBattlesService(
            [NotNull] IBattlesRepository battlesRepository,
            [NotNull] IBattleUnitsService battleUnitsService,
            [NotNull] IBattleDefinitionsService battleDefinitionsService,
            [NotNull] IBattlesCacheService battlesCacheService,
            [NotNull] IUserUnitsService userUnitsService,
            [NotNull] IUsersService usersService)
        {
            BattlesRepository = battlesRepository ?? throw new ArgumentNullException(nameof(battlesRepository));
            BattleUnitsService = battleUnitsService ?? throw new ArgumentNullException(nameof(battleUnitsService));
            BattleDefinitionsService = battleDefinitionsService ?? throw new ArgumentNullException(nameof(battleDefinitionsService));
            BattlesCacheService = battlesCacheService ?? throw new ArgumentNullException(nameof(battlesCacheService));
            UserUnitsService = userUnitsService ?? throw new ArgumentNullException(nameof(userUnitsService));
            UsersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
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

        public async Task<IBattleObject> FindActiveBattleByUserId(Guid userId)
        {
            var battleEntity = await BattlesRepository.FindActiveBattleByUserIdAsync(userId);
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
            
            var battleObject = MutableBattleObject.FromEntity(battleEntity);
            
            var battleInitialUnits = await BattleUnitsService.CreateUnitsFromBattleDefinition(battleDefinition, battleObject.Id);
            battleObject.Units = new List<MutableBattleUnitObject>(battleInitialUnits.Select(MutableBattleUnitObject.CopyFrom));
            
            await FillUsers(battleObject);
            
            return battleObject;
        }

        public async Task<IBattleObject> BeginBattle(Guid userId, IBattleObject battleObject)
        {
            var mutableBattleObject = ToMutableObject(battleObject);
            mutableBattleObject.IsActive = true;
            await BattlesRepository.UpdateBattle(MutableBattleObject.ToEntity(mutableBattleObject));
            
            var userUnits = await UserUnitsService.GetAliveUnitsByUserAsync(userId);
            var userBattleUnits = await BattleUnitsService.CreateUnitsFromUserUnits(userUnits, InBattlePlayerNumber.Player1, battleObject.Id);
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
            battleObject.UsersIds = new List<Guid>(userIds);
        }

        private MutableBattleObject ToMutableObject(IBattleObject battleObject)
        {
            return (MutableBattleObject)battleObject;
        }
    }
}