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

        public Task<IPlayerEntity> FindByName(string playerName)
        {
            var player = _playerEntities.First(x => x.Name == playerName);
            return Task.FromResult<IPlayerEntity>(player);
        }

        public Task<IPlayerEntity[]> GetByIds(IReadOnlyList<Guid> playerId)
        {
            return Task.FromResult(_playerEntities.Where(x => playerId.Contains(x.Id)).ToArray<IPlayerEntity>());
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

        public Task DayIncrement(IReadOnlyList<Guid> playerIds)
        {
            _playerEntities.Where(x => playerIds.Contains(x.Id)).ToList().ForEach(x => x.Day++);
            return Task.CompletedTask;
        }

        public Task StageIncrement(Guid playerId)
        {
            var player = _playerEntities.First(x => x.Id == playerId);
            player.Stage++;
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