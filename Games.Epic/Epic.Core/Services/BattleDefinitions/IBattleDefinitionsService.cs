using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Epic.Core.Services.BattleDefinitions
{
    public interface IBattleDefinitionsService
    {
        Task<int> GetBattlesCountForPlayer(Guid playerId);
        Task<IReadOnlyCollection<IBattleDefinitionObject>> GetActiveBattleDefinitionsByPlayerAsync(Guid playerId);
        
        Task<IBattleDefinitionObject> GetBattleDefinitionByPlayerAndId(Guid playerId, Guid battleDefinitionId);
        
        Task SetFinished(Guid battleDefinitionId);
    }
}