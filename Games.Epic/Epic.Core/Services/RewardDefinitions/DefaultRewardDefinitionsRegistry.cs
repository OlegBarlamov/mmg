using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Data.RewardDefinitions;
using JetBrains.Annotations;

namespace Epic.Core.Services.RewardDefinitions
{
    [UsedImplicitly]
    public class DefaultRewardDefinitionsRegistry : IRewardDefinitionsRegistry
    {
        public IReadOnlyList<IGrouping<string, IRewardDefinitionEntity>> AllOrdered => _allOrdered;
        
        private List<IGrouping<string, IRewardDefinitionEntity>> _allOrdered = new List<IGrouping<string, IRewardDefinitionEntity>>();
        
        public IRewardDefinitionsRepository Repository { get; }

        public DefaultRewardDefinitionsRegistry([NotNull] IRewardDefinitionsRepository repository)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task Load()
        {
            _allOrdered.Clear();
            
            var entities = (await Repository.GetAll()).ToList();
            var grouped = entities.GroupBy(x => x.Key).ToList();
            grouped.Sort((x, y) => 
                x.First().Value.CompareTo(y.First().Value));
            
            _allOrdered.AddRange(grouped);
        }
    }
}