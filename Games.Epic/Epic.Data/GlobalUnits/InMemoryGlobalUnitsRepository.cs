using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Data.PlayerUnits;
using JetBrains.Annotations;
using NetExtensions.Collections;

namespace Epic.Data.GlobalUnits
{
    [UsedImplicitly]
    public class InMemoryGlobalUnitsRepository : IGlobalUnitsRepository
    {
        public string Name => nameof(InMemoryGlobalUnitsRepository);
        public string EntityName => "PlayerUnit";
        
        private readonly List<GlobalUnitEntity> _units = new List<GlobalUnitEntity>();
        
        public Task<IGlobalUnitEntity[]> GetByContainerId(Guid containerId)
        {
            var units = _units.Where(unit => unit.ContainerId == containerId).ToArray<IGlobalUnitEntity>();
            return Task.FromResult(units);
        }
        
        public Task<IGlobalUnitEntity[]> GetAliveByContainerId(Guid containerId)
        {
            var aliveUnits = _units
                .Where(unit => unit.ContainerId == containerId && unit.IsAlive)
                .ToArray<IGlobalUnitEntity>();
            return Task.FromResult(aliveUnits);
        }

        public Task<IGlobalUnitEntity[]> FetchUnitsByIds(IReadOnlyCollection<Guid> ids)
        {
            return Task.FromResult(_units.Where(unit => ids.Contains(unit.Id)).ToArray<IGlobalUnitEntity>());
        }

        public Task RemoveByIds(IEnumerable<Guid> ids)
        {
            foreach (var id in ids)
            {
                _units.Remove(_units.Single(unit => unit.Id == id));
            }
            return Task.CompletedTask;
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
            
            _units.Add(entity);
            
            return Task.FromResult((IGlobalUnitEntity)entity);
        }

        public Task Update(IGlobalUnitEntity[] entities)
        {
            entities.ForEach(entity =>
            {
                var target = _units.First(x => x.Id == entity.Id);
                target.UpdateFrom(entity);
            });
            return Task.CompletedTask;
        }

        public Task<IGlobalUnitEntity> GetAliveUnitFromContainerInSlot(Guid containerId, int slotIndex)
        {
            return Task.FromResult<IGlobalUnitEntity>(_units.First(unit => unit.ContainerId == containerId && unit.ContainerSlotIndex == slotIndex));
        }
    }
}