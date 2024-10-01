using System;
using System.Threading.Tasks;
using Epic.Core.Objects.Battle;

namespace Epic.Core
{
    public interface IBattlesService
    {
        Task<IBattleObject> FindActiveBattleByUserId(Guid userId);
    }
}