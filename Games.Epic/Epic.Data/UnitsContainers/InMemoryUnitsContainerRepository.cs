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
        public string Name => nameof(InMemoryUnitsContainerRepository);
        public string EntityName => "UnitsContainer";
        
        private readonly List<MutableUnitsContainerEntity> _unitsContainers = new List<MutableUnitsContainerEntity>();
        
        public Task<IUnitsContainerEntity> GetById(Guid id)
        {
            return Task.FromResult<IUnitsContainerEntity>(_unitsContainers.First(x => x.Id == id));
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
                _unitsContainers.First(y => y.Id == x.Id).CopyFrom(x); 
            });
            return Task.CompletedTask;
        }
    }
}