using System;
using System.Threading.Tasks;

namespace Epic.Data.Battles
{
    public interface IBattlesRepository : IRepository
    {
        Task<IBattleEntity> GetBattleByIdAsync(Guid id);
        Task<IBattleEntity> FindActiveBattleByUserIdAsync(Guid id);
        Task<IBattleEntity> CreateBattleAsync(Guid battleDefinitionId, Guid[] userIds, int width, int height, bool isActive);
        Task UpdateBattle(IBattleEntity battleEntity);
        Task<Guid[]> GetBattleUsers(Guid battleId);
    }
}