using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epic.Core.Services.Players;

namespace Epic.Core.Services.BattleDefinitions
{
    public interface IBattleDefinitionsService
    {
        Task<int> GetBattlesCountForPlayer(IPlayerObject player);
        Task<IReadOnlyCollection<IBattleDefinitionObject>> GetNotExpiredActiveBattleDefinitionsByPlayerAsync(Guid playerId);
        
        Task<IBattleDefinitionObject> GetBattleDefinitionById(Guid battleDefinitionId);
        Task<IBattleDefinitionObject> GetBattleDefinitionByPlayerAndId(Guid playerId, Guid battleDefinitionId);

        Task<IBattleDefinitionObject> CreateBattleDefinition(
            Guid playerId,
            int width,
            int height,
            int expireAtDay,
            int rewardVisibility,
            int guardVisibility,
            Guid? containerId = null);
        
        Task<IBattleDefinitionObject> CreateBattleDefinition(int width, int height);
        
        Task SetFinished(Guid battleDefinitionId);
    }
}