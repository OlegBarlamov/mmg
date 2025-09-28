using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Epic.Data.RewardDefinitions
{
    [UsedImplicitly]
    public class InMemoryRewardDefinitionsRepository : IRewardDefinitionsRepository
    {
        public string Name => nameof(InMemoryRewardDefinitionsRepository);
        public string EntityName => "RewardDefinition";
        
        private readonly List<RewardDefinitionEntity>  _rewardDefinitions = new List<RewardDefinitionEntity>();
        
        public Task<IRewardDefinitionEntity[]> GetAll()
        {
            return Task.FromResult(_rewardDefinitions.ToArray<IRewardDefinitionEntity>());
        }

        public Task<IRewardDefinitionEntity> Create(IRewardDefinitionFields fields)
        {
            var entity = new RewardDefinitionEntity(Guid.NewGuid(), fields);
            _rewardDefinitions.Add(entity);
            return Task.FromResult<IRewardDefinitionEntity>(entity);
        }

        public Task<IRewardDefinitionEntity[]> CreateBatch(IEnumerable<IRewardDefinitionFields> fields)
        {
            var entities = fields.Select(x => new RewardDefinitionEntity(Guid.NewGuid(), x)).ToArray();
            _rewardDefinitions.AddRange(entities);
            return Task.FromResult(entities.ToArray<IRewardDefinitionEntity>());
        }
    }
}