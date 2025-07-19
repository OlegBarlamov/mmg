using System;
using System.Threading.Tasks;

namespace Epic.Data.Battles
{
    public interface IBattlesRepository : IRepository
    {
        Task<IBattleEntity> GetBattleByIdAsync(Guid id);
        Task<IBattleEntity> FindActiveBattleByPlayerIdAsync(Guid playerId);
        Task<IBattleEntity> CreateBattleAsync(Guid battleDefinitionId, Guid[] playerIds, int width, int height, bool isActive);
        Task UpdateBattle(IBattleEntity battleEntity);
        Task<Guid[]> GetBattleUsers(Guid battleId);
    }
}