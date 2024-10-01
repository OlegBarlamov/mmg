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
            var fetchingUnitsTasks = battleDefinitions.Select(battleDefinition => UserUnitsService
                .GetUnitsByIds(battleDefinition.UnitsIds)
                .ContinueWith(task => battleDefinition.Units = task.Result)
            );
            await Task.WhenAll(fetchingUnitsTasks);

            return battleDefinitions;
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