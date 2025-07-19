using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epic.Core.Objects;
using Epic.Core.Objects.UserUnit;

namespace Epic.Core.Services.Units
{
    public interface IPlayerUnitsService
    {
        Task<IReadOnlyCollection<IPlayerUnitObject>> GetAliveUnitsByPlayer(Guid playerId);
        
        Task<IReadOnlyCollection<IPlayerUnitObject>> GetUnitsByIds(IReadOnlyCollection<Guid> ids);
        
        Task UpdateUnits(IReadOnlyCollection<IPlayerUnitObject> playerUnits);
        
        Task<IReadOnlyCollection<IPlayerUnitObject>> CreateUnits(IReadOnlyCollection<CreatePlayerUnitData> unitsToCreate);
    }
}