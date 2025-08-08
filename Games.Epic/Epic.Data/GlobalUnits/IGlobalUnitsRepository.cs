using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epic.Data.PlayerUnits;

namespace Epic.Data.GlobalUnits
{
    public interface IGlobalUnitsRepository : IRepository
    {
        Task<IGlobalUnitEntity[]> GetByContainerId(Guid containerId);
        
        Task<IGlobalUnitEntity[]> GetAliveByContainerId(Guid containerId);
        
        Task<IGlobalUnitEntity[]> FetchUnitsByIds(IReadOnlyCollection<Guid> ids);
        Task<int> GetUnitsCount(Guid containerId);
        Task RemoveByIds(IEnumerable<Guid> ids);
        
        Task<IGlobalUnitEntity> Create(Guid typeId, int count, Guid containerId, bool isAlive, int containerSlotIndex);
        
        Task Update(params IGlobalUnitEntity[] entities);
        
        Task<IGlobalUnitEntity> GetAliveUnitFromContainerInSlot(Guid containerId, int slotIndex);
    }
}