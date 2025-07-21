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
        
        public Task<IPlayerUnitEntity[]> GetByContainerId(Guid containerId)
        {
            var units = _playerUnits.Where(unit => unit.ContainerId == containerId).ToArray<IPlayerUnitEntity>();
            return Task.FromResult(units);
        }
        
        public Task<IPlayerUnitEntity[]> GetAliveByContainerId(Guid containerId)
        {
            var aliveUnits = _playerUnits
                .Where(unit => unit.ContainerId == containerId && unit.IsAlive)
                .ToArray<IPlayerUnitEntity>();
            return Task.FromResult(aliveUnits);
        }

        public Task<IPlayerUnitEntity[]> FetchUnitsByIds(IReadOnlyCollection<Guid> ids)
        {
            return Task.FromResult(_playerUnits.Where(unit => ids.Contains(unit.Id)).ToArray<IPlayerUnitEntity>());
        }

        public Task<IPlayerUnitEntity> CreatePlayerUnit(Guid typeId, int count, Guid playerId, Guid containerId, bool isAlive, int containerSlotIndex)
        {
            var entity = new PlayerUnitEntity
            {
                Id = Guid.NewGuid(),
                TypeId = typeId,
                Count = count,
                PlayerId = playerId,
                IsAlive = isAlive,
                ContainerId = containerId,
                ContainerSlotIndex = containerSlotIndex,
            };
            
            _playerUnits.Add(entity);
            
            return Task.FromResult((IPlayerUnitEntity)entity);
        }

        public Task Update(IPlayerUnitEntity[] entities)
        {
            entities.ForEach(entity =>
            {
                var target = _playerUnits.First(x => x.Id == entity.Id);
                target.UpdateFrom(entity);
            });
            return Task.CompletedTask;
        }

        public Task<IPlayerUnitEntity> GetAliveUnitFromContainerInSlot(Guid containerId, int slotIndex)
        {
            return Task.FromResult<IPlayerUnitEntity>(_playerUnits.First(unit => unit.ContainerId == containerId && unit.ContainerSlotIndex == slotIndex));
        }
    }
}