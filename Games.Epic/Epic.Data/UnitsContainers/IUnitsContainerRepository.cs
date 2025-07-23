using System;
using System.Threading.Tasks;

namespace Epic.Data.UnitsContainers
{
    public interface IUnitsContainerRepository : IRepository
    {
        Task<IUnitsContainerEntity> GetById(Guid id);
        Task<IUnitsContainerEntity> Create(int capacity, Guid ownerPlayerId);
        Task Update(params IUnitsContainerEntity[] entities);
    }
}