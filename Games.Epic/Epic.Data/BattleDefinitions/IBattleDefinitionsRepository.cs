using System;
using System.Threading.Tasks;

namespace Epic.Data.BattleDefinitions
{
    public interface IBattleDefinitionsRepository : IRepository
    {
        Task<IBattleDefinitionEntity[]> GetActiveBattleDefinitionsByPlayer(Guid playerId, int currentDay);
        Task<IBattleDefinitionEntity> Create(Guid playerId, IBattleDefinitionFields fields);
        Task<IBattleDefinitionEntity> Create(IBattleDefinitionFields fields);
        Task<IBattleDefinitionEntity> GetByPlayerAndId(Guid playerId, Guid battleDefinitionId);
        Task<IBattleDefinitionEntity> GetById(Guid battleDefinitionId);
        Task SetFinished(Guid battleDefinitionId);
        Task<int> CountBattles(Guid playerId, int? day = null);
    }
}
