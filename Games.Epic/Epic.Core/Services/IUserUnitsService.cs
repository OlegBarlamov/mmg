using System;
using System.Threading.Tasks;
using Epic.Core.Objects;

namespace Epic.Core
{
    public interface IUserUnitsService
    {
        Task<IUserUnitObject[]> GetAliveUnitsByUserAsync(Guid userId);
    }
}