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
        
        private readonly List<BattleDefinitionEntity> _battleDefinitions = new List<BattleDefinitionEntity>();
        private readonly List<IPlayerToBattleDefinitionEntity> _playerBattleDefinitions = new List<IPlayerToBattleDefinitionEntity>();

        public Task<IBattleDefinitionEntity[]> GetActiveBattleDefinitionsByPlayer(Guid playerId)
        {
            // Find all user-battle relations for the given user
            var userBattles = _playerBattleDefinitions
                .Where(ub => ub.PlayerId == playerId)
                .Select(ub => ub.BattleDefinitionId)
                .ToList();

            // Find corresponding battle definitions
            var battles = _battleDefinitions
                .Where(bd => !bd.Finished && userBattles.Contains(bd.Id))
                .ToArray<IBattleDefinitionEntity>();

            return Task.FromResult(battles);
        }

        public Task<IBattleDefinitionEntity> Create(Guid playerId, int width, int height, Guid[] unitIds)
        {
            var battleDefinition = new BattleDefinitionEntity
            {
                Id = Guid.NewGuid(),
                Height = height,
                Width = width,
                UnitsIds = unitIds,
                Finished = false,
            };
            _battleDefinitions.Add(battleDefinition);

            var userBattleDefinition = new PlayerToBattleDefinitionEntity
            {
                Id = Guid.NewGuid(),
                BattleDefinitionId = battleDefinition.Id,
                PlayerId = playerId,
            };
            _playerBattleDefinitions.Add(userBattleDefinition);

            return Task.FromResult((IBattleDefinitionEntity)battleDefinition);
        }

        public Task<IBattleDefinitionEntity> GetByPlayerAndId(Guid playerId, Guid battleDefinitionId)
        {
            var userBattleDefinition = _playerBattleDefinitions.FirstOrDefault(x =>
                x.PlayerId == playerId && x.BattleDefinitionId == battleDefinitionId);
            if (userBattleDefinition == null)
                throw new EntityNotFoundException(this, $"PlayerId: {playerId}; BattleDefinitionId: {battleDefinitionId}");

            return Task.FromResult<IBattleDefinitionEntity>(_battleDefinitions.First(x => x.Id == battleDefinitionId));
        }

        public Task SetFinished(Guid battleDefinitionId)
        {
            _battleDefinitions.First(x => x.Id == battleDefinitionId).Finished = true;
            return Task.CompletedTask;
        }

        public Task<int> CountBattles(Guid playerId)
        {
            return Task.FromResult(_playerBattleDefinitions.Count(ub => ub.PlayerId == playerId));
        }
    }
}