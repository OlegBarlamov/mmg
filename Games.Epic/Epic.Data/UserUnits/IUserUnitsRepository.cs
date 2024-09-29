using System;
using System.Threading.Tasks;

namespace Epic.Data.UserUnits
{
    public interface IUserUnitsRepository : IRepository
    {
        Task<IUserUnitEntity[]> GetUnitsByUserAsync(Guid userId);
        
        Task<IUserUnitEntity[]> GetAliveUnitsByUserAsync(Guid userId);
    }
}