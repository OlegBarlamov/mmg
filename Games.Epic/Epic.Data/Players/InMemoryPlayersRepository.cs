using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Epic.Data.Players
{
    [UsedImplicitly]
    public class InMemoryPlayersRepository : IPlayersRepository
    {
        public string Name => nameof(InMemoryPlayersRepository);
        public string EntityName => "Player";
        
        private readonly List<MutablePlayerEntity> _playerEntities = new List<MutablePlayerEntity>();
        
        public Task<IPlayerEntity> GetById(Guid playerId)
        {
            var player = _playerEntities.First(x => x.Id == playerId);
            return Task.FromResult<IPlayerEntity>(player);
        }

        public Task<IPlayerEntity> GetByName(string name)
        {
            return Task.FromResult<IPlayerEntity>(_playerEntities.First(x => x.Name == name));
        }

        public Task<IPlayerEntity[]> GetByUserId(Guid userId)
        {
            var players = _playerEntities.Where(x => x.UserId == userId);
            return Task.FromResult(players.ToArray<IPlayerEntity>());
        }

        public Task Update(IPlayerEntity entity)
        {
            var targetPlayer = _playerEntities.First(x => x.Id == entity.Id);
            targetPlayer.UpdateFrom(entity);
            return Task.CompletedTask;
        }

        public Task<IPlayerEntity> Create(IPlayerEntityFields fields)
        {
            var newPlayer = MutablePlayerEntity.FromFields(Guid.NewGuid(), fields);
            _playerEntities.Add(newPlayer);
            
            return Task.FromResult<IPlayerEntity>(newPlayer);
        }

        public Task SetDefeated(Guid[] playerIds)
        {
            _playerEntities.Where(x => playerIds.Contains(x.Id)).ToList().ForEach(x => x.IsDefeated = true);
            return Task.CompletedTask;
        }

        public Task DayIncrement(Guid[] playerIds)
        {
            _playerEntities.Where(x => playerIds.Contains(x.Id)).ToList().ForEach(x => x.Day++);
            return Task.CompletedTask;
        }

        public Task SetGenerationInProgress(Guid[] playerIds, bool isGenerationInProgress)
        {
            _playerEntities.Where(x => playerIds.Contains(x.Id)).ToList().ForEach(x => x.GenerationInProgress = isGenerationInProgress);
            return Task.CompletedTask;
        }

        public Task SetActiveHero(Guid playerId, Guid heroId)
        {
            var targetPlayer = _playerEntities.First(x => x.Id == playerId);
            targetPlayer.ActiveHeroId = heroId;
            return Task.CompletedTask;
        }
    }
}