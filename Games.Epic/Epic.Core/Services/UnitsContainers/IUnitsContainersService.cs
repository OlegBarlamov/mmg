using System;
using System.Threading.Tasks;

namespace Epic.Core.Services.UnitsContainers
{
    public interface IUnitsContainersService
    {
        Task<IUnitsContainerObject> GetById(Guid id);
        Task<IUnitsContainerObject> Create(int capacity);
    }
}