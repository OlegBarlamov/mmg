using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Data.Exceptions;

namespace Epic.Data.BattleDefinitions
{
    public class InMemoryBattleDefinitionsRepository : IBattleDefinitionsRepository
    {
        public string Name => nameof(InMemoryBattleDefinitionsRepository);
        public string EntityName => "BattleDefinition";
        
        private readonly List<IBattleDefinitionEntity> _battleDefinitions = new List<IBattleDefinitionEntity>();
        private readonly List<IUserBattleDefinitionEntity> _userBattleDefinitions = new List<IUserBattleDefinitionEntity>();

        public Task<IBattleDefinitionEntity[]> GetBattleDefinitionsByUserAsync(Guid userId)
        {
            // Find all user-battle relations for the given user
            var userBattles = _userBattleDefinitions
                .Where(ub => ub.UserId == userId)
                .Select(ub => ub.BattleDefinitionId)
                .ToList();

            // Find corresponding battle definitions
            var battles = _battleDefinitions
                .Where(bd => userBattles.Contains(bd.Id))
                .ToArray();

            return Task.FromResult(battles);
        }

        public Task<IBattleDefinitionEntity> CreateBattleDefinitionAsync(Guid userId, int width, int height, Guid[] unitIds)
        {
            var battleDefinition = new BattleDefinitionEntity
            {
                Id = Guid.NewGuid(),
                Height = height,
                Width = width,
                UnitsIds = unitIds,
            };
            _battleDefinitions.Add(battleDefinition);

            var userBattleDefinition = new UserBattleDefinitionEntity
            {
                Id = Guid.NewGuid(),
                BattleDefinitionId = battleDefinition.Id,
                UserId = userId,
            };
            _userBattleDefinitions.Add(userBattleDefinition);

            return Task.FromResult((IBattleDefinitionEntity)battleDefinition);
        }

        public Task<IBattleDefinitionEntity> GetBattleDefinitionByUserAndId(Guid userId, Guid battleDefinitionId)
        {
            var userBattleDefinition = _userBattleDefinitions.FirstOrDefault(x =>
                x.UserId == userId && x.BattleDefinitionId == battleDefinitionId);
            if (userBattleDefinition == null)
                throw new EntityNotFoundException(this, $"UserId: {userId}; BattleDefinitionId: {battleDefinitionId}");

            return Task.FromResult(_battleDefinitions.First(x => x.Id == battleDefinitionId));
        }
    }
}