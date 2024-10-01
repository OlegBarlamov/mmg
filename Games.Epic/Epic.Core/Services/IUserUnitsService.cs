using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epic.Core.Objects;

namespace Epic.Core
{
    public interface IUserUnitsService
    {
        Task<IReadOnlyCollection<IUserUnitObject>> GetAliveUnitsByUserAsync(Guid userId);
        
        Task<IReadOnlyCollection<IUserUnitObject>> GetUnitsByIds(IReadOnlyCollection<Guid> ids);
    }
}