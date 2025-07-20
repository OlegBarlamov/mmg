using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

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

        public Task<IUnitsContainerEntity> Create(int capacity)
        {
            var instance = new MutableUnitsContainerEntity
            {
                Id = Guid.NewGuid(),
                Capacity = capacity
            };
            _unitsContainers.Add(instance);
            
            return Task.FromResult<IUnitsContainerEntity>(instance);
        }
    }
}