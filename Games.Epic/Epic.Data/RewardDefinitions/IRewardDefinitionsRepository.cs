using System.Collections.Generic;
using System.Threading.Tasks;

namespace Epic.Data.RewardDefinitions
{
    public interface IRewardDefinitionsRepository : IRepository
    {
        Task<IRewardDefinitionEntity[]> GetAll();
        
        Task<IRewardDefinitionEntity> Create(IRewardDefinitionFields fields);
        Task<IRewardDefinitionEntity[]> CreateBatch(IEnumerable<IRewardDefinitionFields> fields);
    }
}