using System;
using System.Threading.Tasks;

namespace Epic.Data.UnitsContainers
{
    public interface IUnitsContainerRepository
    {
        Task<IUnitsContainerEntity> GetById(Guid id);
        Task<IUnitsContainerEntity> Create(int capacity, Guid ownerPlayerId);
        Task Update(params IUnitsContainerEntity[] entities);
    }
}