using System;
using System.Threading.Tasks;
using Epic.Core.Objects.Battle;
using Epic.Data.Battles;
using JetBrains.Annotations;

namespace Epic.Core
{
    [UsedImplicitly]
    public class DefaultBattlesService : IBattlesService
    {
        public IBattlesRepository BattlesRepository { get; }
        public IBattleUnitsService BattleUnitsService { get; }

        public DefaultBattlesService([NotNull] IBattlesRepository battlesRepository, [NotNull] IBattleUnitsService battleUnitsService)
        {
            BattlesRepository = battlesRepository ?? throw new ArgumentNullException(nameof(battlesRepository));
            BattleUnitsService = battleUnitsService ?? throw new ArgumentNullException(nameof(battleUnitsService));
        }
        
        public async Task<IBattleObject> FindActiveBattleByUserId(Guid userId)
        {
            var battleEntity = await BattlesRepository.FindActiveBattleByUserIdAsync(userId);
            if (battleEntity == null)
                return null;

            var battleObject = ToBattleObject(battleEntity);
            await FillUnits(battleObject);
            return battleObject;
        }

        private async Task FillUnits(MutableBattleObject battleObject)
        {
            battleObject.Units = await BattleUnitsService.GetBattleUnits(battleObject.Id);
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
                Units = null
            };
        }
    }
}