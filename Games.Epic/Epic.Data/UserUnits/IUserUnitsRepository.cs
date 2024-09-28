using System;
using System.Threading.Tasks;

namespace Epic.Data.UserUnits
{
    public interface IUserUnitsRepository
    {
        Task<IUserUnitEntity[]> GetUnitsByUserAsync(Guid userId);
    }
}