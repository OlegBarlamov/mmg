using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Epic.Core
{
    public interface IBattleDefinitionsService
    {
        Task<IReadOnlyCollection<IBattleDefinitionObject>> GetActiveBattleDefinitionsByUserAsync(Guid userId);
        
        Task<IBattleDefinitionObject> GetBattleDefinitionByUserAndId(Guid userId, Guid battleDefinitionId);
        
        Task SetFinished(Guid battleDefinitionId);
    }
}