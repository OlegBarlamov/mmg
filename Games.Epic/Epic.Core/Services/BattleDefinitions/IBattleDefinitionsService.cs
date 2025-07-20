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

        Task<IBattleDefinitionObject> CreateBattleDefinition(Guid playerId, int width, int height);
        
        Task SetFinished(Guid battleDefinitionId);
    }
}