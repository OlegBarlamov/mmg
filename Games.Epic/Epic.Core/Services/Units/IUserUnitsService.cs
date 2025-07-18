using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epic.Core.Objects;
using Epic.Core.Objects.UserUnit;

namespace Epic.Core.Services.Units
{
    public interface IUserUnitsService
    {
        Task<IReadOnlyCollection<IUserUnitObject>> GetAliveUnitsByUserAsync(Guid userId);
        
        Task<IReadOnlyCollection<IUserUnitObject>> GetUnitsByIds(IReadOnlyCollection<Guid> ids);
        
        Task UpdateUnits(IReadOnlyCollection<IUserUnitObject> userUnits, bool updateCache = false);
        
        Task<IReadOnlyCollection<IUserUnitObject>> CreateUnits(IReadOnlyCollection<CreateUserUnitData> unitsToCreate);
    }
}