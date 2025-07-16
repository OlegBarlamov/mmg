using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Epic.Data.BattleDefinitions
{
    public interface IBattleDefinitionsRepository : IRepository
    {
        Task<IBattleDefinitionEntity[]> GetActiveBattleDefinitionsByUserAsync(Guid userId);
        Task<IBattleDefinitionEntity> CreateBattleDefinitionAsync(Guid userId, int width, int height, Guid[] unitIds);
        Task<IBattleDefinitionEntity> GetBattleDefinitionByUserAndId(Guid userId, Guid battleDefinitionId);
        Task SetFinished(Guid battleDefinitionId);
    }
}