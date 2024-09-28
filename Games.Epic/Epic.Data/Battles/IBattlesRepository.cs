using System;
using System.Threading.Tasks;

namespace Epic.Data.Battles
{
    public interface IBattlesRepository
    {
        Task<IBattleEntity> GetBattleByIdAsync(Guid id);
        Task<IBattleEntity> GetActiveBattleByUserIdAsync(Guid id);
    }
}