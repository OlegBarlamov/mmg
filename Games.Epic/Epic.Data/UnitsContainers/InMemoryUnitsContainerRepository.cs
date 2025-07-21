using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NetExtensions.Collections;

namespace Epic.Data.UnitsContainers
{
    [UsedImplicitly]
    public class InMemoryUnitsContainerRepository : IUnitsContainerRepository
    {
        private readonly List<IUnitsContainerEntity> _unitsContainers = new List<IUnitsContainerEntity>();
        
        public Task<IUnitsContainerEntity> GetById(Guid id)
        {
            return Task.FromResult(_unitsContainers.First(x => x.Id == id));
        }

        public Task<IUnitsContainerEntity> Create(int capacity, Guid ownerPlayerId)
        {
            var instance = new MutableUnitsContainerEntity
            {
                Id = Guid.NewGuid(),
                Capacity = capacity,
                OwnerPlayerId = ownerPlayerId,
            };
            _unitsContainers.Add(instance);
            
            return Task.FromResult<IUnitsContainerEntity>(instance);
        }

        public Task Update(params IUnitsContainerEntity[] entities)
        {
            entities.ForEach(x =>
            {
                _unitsContainers.First(y => y.Id == x.Id).OwnerPlayerId = x.OwnerPlayerId;
            });
            return Task.CompletedTask;
        }
    }
}