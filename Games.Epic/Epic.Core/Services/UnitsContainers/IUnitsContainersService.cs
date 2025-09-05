using System;
using System.Threading.Tasks;

namespace Epic.Core.Services.UnitsContainers
{
    public interface IUnitsContainersService
    {
        Task<IUnitsContainerObject> GetById(Guid id);
        Task<IUnitsContainerObject> Create(int capacity, Guid ownerPlayerId);
        Task<IUnitsContainerObject> ChangeOwner(IUnitsContainerObject container, Guid ownerPlayerId);
        Task<IUnitsContainerObject> ChangeCapacity(IUnitsContainerObject container, int newCapacity);
    }
}