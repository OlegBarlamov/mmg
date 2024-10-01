using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Epic.Data.UserUnits
{
    public interface IUserUnitsRepository : IRepository
    {
        Task<IUserUnitEntity[]> GetUnitsByUserAsync(Guid userId);
        
        Task<IUserUnitEntity[]> GetAliveUnitsByUserAsync(Guid userId);
        
        Task<IUserUnitEntity[]> FetchUnitsByIds(IReadOnlyCollection<Guid> ids);
        
        Task<IUserUnitEntity> CreateUserUnit(Guid typeId, int count, Guid userId, bool isAlive);
    }
}