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

        public Task<IPlayerEntity[]> GetByUserId(Guid userId)
        {
            var players = _playerEntities.Where(x => x.UserId == userId);
            return Task.FromResult(players.ToArray<IPlayerEntity>());
        }

        public Task Update(IPlayerEntity entity)
        {
            var targetPlayer = _playerEntities.First(x => x.Id == entity.Id);
            targetPlayer.Name = entity.Name;
            targetPlayer.UserId = entity.UserId;
            targetPlayer.IsDefeated = entity.IsDefeated;
            targetPlayer.PlayerType = entity.PlayerType;
            targetPlayer.Day = entity.Day;
            return Task.CompletedTask;
        }

        public Task<IPlayerEntity> Create(IPlayerEntityFields fields)
        {
            var newPlayer = new MutablePlayerEntity
            {
                Id = Guid.NewGuid(),
                Name = fields.Name,
                UserId = fields.UserId,
                IsDefeated = fields.IsDefeated,
                PlayerType = fields.PlayerType,
                Day = fields.Day,
            };
            _playerEntities.Add(newPlayer);
            
            return Task.FromResult<IPlayerEntity>(newPlayer);
        }
    }
}