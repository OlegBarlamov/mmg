using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Epic.Core.Services.Units
{
    public interface IPlayerUnitsService
    {
        Task<IReadOnlyCollection<IPlayerUnitObject>> GetAliveUnitsByContainerId(Guid containerId);
        
        Task<IReadOnlyCollection<IPlayerUnitObject>> GetUnitsByIds(IReadOnlyCollection<Guid> ids);
        
        Task UpdateUnits(IReadOnlyCollection<IPlayerUnitObject> playerUnits);
        
        Task<IReadOnlyCollection<IPlayerUnitObject>> CreateUnits(IReadOnlyCollection<CreatePlayerUnitData> unitsToCreate);
    }
}