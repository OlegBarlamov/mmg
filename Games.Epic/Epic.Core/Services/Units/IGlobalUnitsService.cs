using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Epic.Core.Services.Units
{
    public interface IGlobalUnitsService
    {
        Task<IGlobalUnitObject> GetById(Guid id);
        Task<bool> HasAliveUnits(Guid containerId);
        Task<int> CountAliveUnits(Guid containerId);
        Task<IReadOnlyCollection<IGlobalUnitObject>> GetAliveUnitsByContainerId(Guid containerId);
        
        Task<IReadOnlyCollection<IGlobalUnitObject>> GetUnitsByIds(IReadOnlyCollection<Guid> ids);
        
        Task UpdateUnits(IReadOnlyCollection<IGlobalUnitObject> playerUnits);
        
        Task<IReadOnlyCollection<IGlobalUnitObject>> CreateUnits(IReadOnlyCollection<CreateUnitData> unitsToCreate);
        Task<IReadOnlyCollection<IGlobalUnitObject>> UpgradeUnits(IReadOnlyCollection<UpgradeUnitData> unitsToUpgrade);
        Task<IGlobalUnitObject> GetAliveUnitFromContainerInSlot(Guid containerId, int slotIndex);
        Task<IReadOnlyList<IGlobalUnitObject>> GetAliveUnitFromContainerPerSlots(Guid containerId, int startSlot, int endSlot);

        Task RemoveUnits(IReadOnlyCollection<IGlobalUnitObject> units);
    }
}