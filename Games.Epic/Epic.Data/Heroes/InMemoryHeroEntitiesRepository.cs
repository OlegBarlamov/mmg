using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Epic.Data.Heroes
{
    [UsedImplicitly]
    public class InMemoryHeroEntitiesRepository : IHeroEntitiesRepository
    {
        public string Name => nameof(InMemoryHeroEntitiesRepository);
        public string EntityName => "Hero";
        
        private readonly List<IHeroEntity> _heroEntities = new List<IHeroEntity>();
        private readonly List<PlayerToHeroEntity> _playerToHeroEntities = new List<PlayerToHeroEntity>();
        
        public Task<IHeroEntity> GetById(Guid id)
        {
            return Task.FromResult(_heroEntities.First(x => x.Id == id));
        }

        public Task<IHeroEntity> Create(Guid id, IHeroEntityFields fieds)
        {
            var entity = MutableHeroEntity.FromFields(id, fieds);
            _heroEntities.Add(entity);
            return Task.FromResult<IHeroEntity>(entity);
        }

        public Task<Guid> GetPlayerByHeroId(Guid heroId)
        {
            var entity = _playerToHeroEntities.First(x => x.HeroId == heroId);
            return Task.FromResult(entity.PlayerId);
        }

        public async Task<IHeroEntity> CreateForPlayer(Guid id, IHeroEntityFields fields, Guid playerId)
        {
            var entity = await Create(id, fields);
            _playerToHeroEntities.Add(new PlayerToHeroEntity
            {
                PlayerId = playerId,
                HeroId = entity.Id,
            });
            return entity;
        }

        public Task GiveToPlayer(Guid heroId, Guid playerId)
        {
            var existing = _playerToHeroEntities.FirstOrDefault(x => x.HeroId == heroId);
            if (existing != null)
            {
                existing.PlayerId = playerId;
                return Task.CompletedTask;
            }

            _playerToHeroEntities.Add(new PlayerToHeroEntity
            {
                PlayerId = playerId,
                HeroId = heroId,
            });
            
            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<IHeroEntity>> GetByPlayerId(Guid playerId)
        {
            var playerToHeroEntities = _playerToHeroEntities.Where(x => x.PlayerId == playerId);
            return Task.FromResult<IReadOnlyCollection<IHeroEntity>>(playerToHeroEntities
                .Select(x => _heroEntities.First(h => h.Id == x.HeroId))
                .ToArray());
        }

        private class PlayerToHeroEntity
        {
            public Guid PlayerId { get; set; }
            public Guid HeroId { get; set; }
        }
    }
}
