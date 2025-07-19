using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Epic.Data.PlayerUnits
{
    public interface IPlayerUnitsRepository : IRepository
    {
        Task<IPlayerUnitEntity[]> GetByPlayer(Guid playerId);
        
        Task<IPlayerUnitEntity[]> GetAliveByPlayer(Guid playerId);
        
        Task<IPlayerUnitEntity[]> FetchUnitsByIds(IReadOnlyCollection<Guid> ids);
        
        Task<IPlayerUnitEntity> CreatePlayerUnit(Guid typeId, int count, Guid playerId, bool isAlive);
        
        Task Update(IPlayerUnitEntity[] entities);
    }
}