using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Epic.Core.Services.Units
{
    public interface IGlobalUnitsService
    {
        Task<IGlobalUnitObject> GetById(Guid id);
        Task<bool> HasAliveUnits(Guid containerId);
        Task<IReadOnlyCollection<IGlobalUnitObject>> GetAliveUnitsByContainerId(Guid containerId);
        
        Task<IReadOnlyCollection<IGlobalUnitObject>> GetUnitsByIds(IReadOnlyCollection<Guid> ids);
        
        Task UpdateUnits(IReadOnlyCollection<IGlobalUnitObject> playerUnits);
        
        Task<IReadOnlyCollection<IGlobalUnitObject>> CreateUnits(IReadOnlyCollection<CreateUnitData> unitsToCreate);
        Task<IGlobalUnitObject> GetAliveUnitFromContainerInSlot(Guid containerId, int slotIndex);
    }
}