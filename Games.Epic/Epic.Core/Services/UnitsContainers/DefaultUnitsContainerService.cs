using System;
using System.Threading.Tasks;
using Epic.Data.UnitsContainers;
using JetBrains.Annotations;

namespace Epic.Core.Services.UnitsContainers
{
    [UsedImplicitly]
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

        public async Task<IUnitsContainerObject> Create(int capacity, Guid ownerPlayerId)
        {
            var entity = await UnitsContainerRepository.Create(capacity, ownerPlayerId);
            return MutableUnitsContainerObject.FromEntity(entity);
        }

        public async Task<IUnitsContainerObject> ChangeOwner(IUnitsContainerObject container, Guid ownerPlayerId)
        {
            var entity = container.ToEntity();
            entity.OwnerPlayerId = ownerPlayerId;

            await UnitsContainerRepository.Update(entity);

            return MutableUnitsContainerObject.FromEntity(entity);
        }

        public async Task<IUnitsContainerObject> ChangeCapacity(IUnitsContainerObject container, int newCapacity)
        {
            var entity = container.ToEntity();
            entity.Capacity = newCapacity;

            await UnitsContainerRepository.Update(entity);

            return MutableUnitsContainerObject.FromEntity(entity);
        }
    }
}