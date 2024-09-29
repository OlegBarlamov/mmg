using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epic.Data.UserUnits
{
    public class InMemoryUserUnitsRepository : IUserUnitsRepository
    {
        public string Name => nameof(InMemoryUserUnitsRepository);
        public string EntityName => "UserUnit";
        
        private readonly List<IUserUnitEntity> _userUnits = new List<IUserUnitEntity>();
        
        public Task<IUserUnitEntity[]> GetUnitsByUserAsync(Guid userId)
        {
            var units = _userUnits.Where(unit => unit.UserId == userId).ToArray();
            return Task.FromResult(units);
        }
        
        public Task<IUserUnitEntity[]> GetAliveUnitsByUserAsync(Guid userId)
        {
            var aliveUnits = _userUnits
                .Where(unit => unit.UserId == userId && unit.IsAlive)
                .ToArray();
            return Task.FromResult(aliveUnits);
        }
    }
}