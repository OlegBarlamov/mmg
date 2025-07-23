using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Data.GlobalUnits;
using JetBrains.Annotations;
using NetExtensions.Collections;

namespace Epic.Data.PlayerUnits
{
    [UsedImplicitly]
    public class InMemoryGlobalUnitsRepository : IGlobalUnitsRepository
    {
        public string Name => nameof(InMemoryGlobalUnitsRepository);
        public string EntityName => "PlayerUnit";
        
        private readonly List<GlobalUnitEntity> _playerUnits = new List<GlobalUnitEntity>();
        
        public Task<IGlobalUnitEntity[]> GetByContainerId(Guid containerId)
        {
            var units = _playerUnits.Where(unit => unit.ContainerId == containerId).ToArray<IGlobalUnitEntity>();
            return Task.FromResult(units);
        }
        
        public Task<IGlobalUnitEntity[]> GetAliveByContainerId(Guid containerId)
        {
            var aliveUnits = _playerUnits
                .Where(unit => unit.ContainerId == containerId && unit.IsAlive)
                .ToArray<IGlobalUnitEntity>();
            return Task.FromResult(aliveUnits);
        }

        public Task<IGlobalUnitEntity[]> FetchUnitsByIds(IReadOnlyCollection<Guid> ids)
        {
            return Task.FromResult(_playerUnits.Where(unit => ids.Contains(unit.Id)).ToArray<IGlobalUnitEntity>());
        }

        public Task<IGlobalUnitEntity> Create(Guid typeId, int count, Guid containerId, bool isAlive, int containerSlotIndex)
        {
            var entity = new GlobalUnitEntity
            {
                Id = Guid.NewGuid(),
                TypeId = typeId,
                Count = count,
                IsAlive = isAlive,
                ContainerId = containerId,
                ContainerSlotIndex = containerSlotIndex,
            };
            
            _playerUnits.Add(entity);
            
            return Task.FromResult((IGlobalUnitEntity)entity);
        }

        public Task Update(IGlobalUnitEntity[] entities)
        {
            entities.ForEach(entity =>
            {
                var target = _playerUnits.First(x => x.Id == entity.Id);
                target.UpdateFrom(entity);
            });
            return Task.CompletedTask;
        }

        public Task<IGlobalUnitEntity> GetAliveUnitFromContainerInSlot(Guid containerId, int slotIndex)
        {
            return Task.FromResult<IGlobalUnitEntity>(_playerUnits.First(unit => unit.ContainerId == containerId && unit.ContainerSlotIndex == slotIndex));
        }
    }
}