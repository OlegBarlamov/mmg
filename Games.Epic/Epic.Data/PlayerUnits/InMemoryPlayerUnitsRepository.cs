using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NetExtensions.Collections;

namespace Epic.Data.PlayerUnits
{
    [UsedImplicitly]
    public class InMemoryPlayerUnitsRepository : IPlayerUnitsRepository
    {
        public string Name => nameof(InMemoryPlayerUnitsRepository);
        public string EntityName => "PlayerUnit";
        
        private readonly List<PlayerUnitEntity> _playerUnits = new List<PlayerUnitEntity>();
        
        public Task<IPlayerUnitEntity[]> GetByPlayer(Guid playerId)
        {
            var units = _playerUnits.Where(unit => unit.PlayerId == playerId).ToArray<IPlayerUnitEntity>();
            return Task.FromResult(units);
        }
        
        public Task<IPlayerUnitEntity[]> GetAliveByPlayer(Guid playerId)
        {
            var aliveUnits = _playerUnits
                .Where(unit => unit.PlayerId == playerId && unit.IsAlive)
                .ToArray<IPlayerUnitEntity>();
            return Task.FromResult(aliveUnits);
        }

        public Task<IPlayerUnitEntity[]> FetchUnitsByIds(IReadOnlyCollection<Guid> ids)
        {
            return Task.FromResult(_playerUnits.Where(unit => ids.Contains(unit.Id)).ToArray<IPlayerUnitEntity>());
        }

        public Task<IPlayerUnitEntity> CreatePlayerUnit(Guid typeId, int count, Guid playerId, bool isAlive)
        {
            var entity = new PlayerUnitEntity
            {
                Id = Guid.NewGuid(),
                TypeId = typeId,
                Count = count,
                PlayerId = playerId,
                IsAlive = isAlive,
            };
            
            _playerUnits.Add(entity);
            
            return Task.FromResult((IPlayerUnitEntity)entity);
        }

        public Task Update(IPlayerUnitEntity[] entities)
        {
            entities.ForEach(entity =>
            {
                var target = _playerUnits.First(x => x.Id == entity.Id);
                target.Count = entity.Count;
                target.PlayerId = entity.PlayerId;
                target.IsAlive = entity.IsAlive;
                target.TypeId = entity.TypeId;
            });
            return Task.CompletedTask;
        }
    }
}