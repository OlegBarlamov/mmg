using System;
using System.Threading.Tasks;

namespace Epic.Data.BattleDefinitions
{
    public interface IBattleDefinitionsRepository : IRepository
    {
        Task<IBattleDefinitionEntity[]> GetActiveBattleDefinitionsByPlayer(Guid playerId);
        Task<IBattleDefinitionEntity> Create(Guid playerId, int width, int height, Guid[] unitIds);
        Task<IBattleDefinitionEntity> GetByPlayerAndId(Guid playerId, Guid battleDefinitionId);
        Task SetFinished(Guid battleDefinitionId);
    }
}
