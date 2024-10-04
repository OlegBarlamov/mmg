using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Data.BattleDefinitions;
using JetBrains.Annotations;

namespace Epic.Core
{
    [UsedImplicitly]
    public class DefaultBattleDefinitionsService : IBattleDefinitionsService
    {
        public IBattleDefinitionsRepository BattleDefinitionsRepository { get; }
        public IUserUnitsService UserUnitsService { get; }

        public DefaultBattleDefinitionsService([NotNull] IBattleDefinitionsRepository battleDefinitionsRepository,
            [NotNull] IUserUnitsService userUnitsService)
        {
            BattleDefinitionsRepository = battleDefinitionsRepository ?? throw new ArgumentNullException(nameof(battleDefinitionsRepository));
            UserUnitsService = userUnitsService ?? throw new ArgumentNullException(nameof(userUnitsService));
        }
        
        public async Task<IReadOnlyCollection<IBattleDefinitionObject>> GetBattleDefinitionsByUserAsync(Guid userId)
        {
            var entities = await BattleDefinitionsRepository.GetBattleDefinitionsByUserAsync(userId);
            var battleDefinitions = entities.Select(ToBattleDefinitionObject).ToArray();
            await Task.WhenAll(battleDefinitions.Select(FillBattleDefinitionObject));
            return battleDefinitions;
        }

        public async Task<IBattleDefinitionObject> GetBattleDefinitionByUserAndId(Guid userId, Guid battleDefinitionId)
        {
            var battleDefinition =
                await BattleDefinitionsRepository.GetBattleDefinitionByUserAndId(userId, battleDefinitionId);
            var battleDefinitionObject = ToBattleDefinitionObject(battleDefinition);
            await FillBattleDefinitionObject(battleDefinitionObject);
            return battleDefinitionObject;
        }

        private Task FillBattleDefinitionObject(MutableBattleDefinitionObject battleDefinitionObject)
        {
            return UserUnitsService
                .GetUnitsByIds(battleDefinitionObject.UnitsIds)
                .ContinueWith(task => battleDefinitionObject.Units = task.Result);
        }

        private MutableBattleDefinitionObject ToBattleDefinitionObject(IBattleDefinitionEntity entity)
        {
            return new MutableBattleDefinitionObject
            {
                Id = entity.Id,
                Width = entity.Width,
                Height = entity.Height,
                UnitsIds = entity.UnitsIds,
            };
        }
    }
}