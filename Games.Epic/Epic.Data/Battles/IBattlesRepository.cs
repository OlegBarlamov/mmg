using System;
using System.Threading.Tasks;

namespace Epic.Data.Battles
{
    public interface IBattlesRepository : IRepository
    {
        Task<IBattleEntity> GetBattleByIdAsync(Guid id);
        Task<IBattleEntity> FindActiveBattleByUserIdAsync(Guid id);
    }
}