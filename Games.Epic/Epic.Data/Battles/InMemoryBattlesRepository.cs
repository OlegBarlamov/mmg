using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Epic.Data.Battles
{
    [UsedImplicitly]
    public class InMemoryBattlesRepository : IBattlesRepository
    {
        public string Name => nameof(InMemoryBattlesRepository);
        public string EntityName => "Battle";
        
        private readonly List<IBattleEntity> _battles = new List<IBattleEntity>();
        private readonly List<IUserBattleEntity> _userBattles = new List<IUserBattleEntity>();

        public Task<IBattleEntity> GetBattleByIdAsync(Guid id)
        {
            var battle = _battles.FirstOrDefault(b => b.Id == id);
            return Task.FromResult(battle);
        }

        public Task<IBattleEntity> FindActiveBattleByUserIdAsync(Guid userId)
        {
            var userBattle = _userBattles.FirstOrDefault(ub => ub.UserId == userId);
            if (userBattle != null)
            {
                var battle = _battles.FirstOrDefault(b => b.Id == userBattle.BattleId && b.IsActive);
                return Task.FromResult(battle);
            }

            return Task.FromResult<IBattleEntity>(null);
        }
    }
}