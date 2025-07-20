using System;
using System.Threading.Tasks;
using Epic.Data.UnitsContainers;
using JetBrains.Annotations;

namespace Epic.Core.Services.UnitsContainers
{
    public class DefaultUnitsContainerService : IUnitsContainersService
    {
        public IUnitsContainerRepository UnitsContainerRepository { get; }

        public DefaultUnitsContainerService([NotNull] IUnitsContainerRepository unitsContainerRepository)
        {
            UnitsContainerRepository = unitsContainerRepository ?? throw new ArgumentNullException(nameof(unitsContainerRepository));
        }
        
        public async Task<IUnitsContainerObject> GetById(Guid id)
        {
            var entity = await UnitsContainerRepository.GetById(id);
            return MutableUnitsContainerObject.FromEntity(entity);
        }

        public async Task<IUnitsContainerObject> Create(int capacity)
        {
            var entity = await UnitsContainerRepository.Create(capacity);
            return MutableUnitsContainerObject.FromEntity(entity);
        }
    }
}