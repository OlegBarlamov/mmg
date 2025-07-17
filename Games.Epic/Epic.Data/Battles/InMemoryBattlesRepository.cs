using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Data.Exceptions;
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
        private readonly List<IBattleDefinitionToBattleEntity> _battleDefinitionToBattleEntities = new List<IBattleDefinitionToBattleEntity>();

        public Task<IBattleEntity> GetBattleByIdAsync(Guid id)
        {
            var battle = _battles.FirstOrDefault(b => b.Id == id);
            return Task.FromResult(battle);
        }

        public Task<IBattleEntity> FindActiveBattleByUserIdAsync(Guid userId)
        {
            var userBattles = _userBattles.Where(ub => ub.UserId == userId)
                .Select(ub => ub.BattleId)
                .ToArray();
            
            if (userBattles.Length > 0)
            {
                var battle = _battles.FirstOrDefault(b => userBattles.Contains(b.Id) && b.IsActive);
                return Task.FromResult(battle);
            }

            return Task.FromResult<IBattleEntity>(null);
        }

        public Task<IBattleEntity> CreateBattleAsync(Guid battleDefinitionId, Guid[] userIds, int width, int height, bool isActive)
        {
            if (_battleDefinitionToBattleEntities.Any(x => x.BattleDefinitionId == battleDefinitionId))
                throw new InvalidOperationException($"Battle with definition {battleDefinitionId} already exists");
            
            var newBattleId = Guid.NewGuid();
            var battleDefinitionToBattleRelation = new BattleDefinitionToBattleEntity
            {
                Id = Guid.NewGuid(),
                BattleDefinitionId = battleDefinitionId,
                BattleId = newBattleId,
            };
            _battleDefinitionToBattleEntities.Add(battleDefinitionToBattleRelation);

            var battleEntity = new MutableBattleEntity
            {
                Id = newBattleId,
                BattleDefinitionId = battleDefinitionId,
                Width = width,
                Height = height,
                IsActive = isActive,
                TurnIndex = 0,
            };
            _battles.Add(battleEntity);

            var userRelations = userIds.Select(id => new UserBattleEntity
            {
                Id = Guid.NewGuid(),
                BattleId = newBattleId,
                UserId = id,
            });
            _userBattles.AddRange(userRelations);

            return Task.FromResult((IBattleEntity)battleEntity);
        }

        public Task UpdateBattle(IBattleEntity battleEntity)
        {
            var battleInstance = _battles.Find(x => x.Id == battleEntity.Id);
            if (battleInstance == null)
                throw new EntityNotFoundException(this, "Battle with id " + battleEntity.Id + " not found for update");

            var mutableBattleInstance = (MutableBattleEntity)battleInstance;
            mutableBattleInstance.Width = battleEntity.Width;
            mutableBattleInstance.Height = battleEntity.Height;
            mutableBattleInstance.IsActive = battleEntity.IsActive;
            mutableBattleInstance.TurnIndex = battleEntity.TurnIndex;
            
            return Task.CompletedTask;
        }

        public Task<Guid[]> GetBattleUsers(Guid battleId)
        {
            var userIds = _userBattles.Where(x => x.BattleId == battleId).Select(x => x.UserId).ToArray();
            return Task.FromResult(userIds);
        }
    }
}