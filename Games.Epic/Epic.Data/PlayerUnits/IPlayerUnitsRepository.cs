using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Epic.Data.PlayerUnits
{
    public interface IPlayerUnitsRepository : IRepository
    {
        Task<IPlayerUnitEntity[]> GetByContainerId(Guid containerId);
        
        Task<IPlayerUnitEntity[]> GetAliveByContainerId(Guid containerId);
        
        Task<IPlayerUnitEntity[]> FetchUnitsByIds(IReadOnlyCollection<Guid> ids);
        
        Task<IPlayerUnitEntity> CreatePlayerUnit(Guid typeId, int count, Guid playerId, Guid containerId, bool isAlive, int containerSlotIndex);
        
        Task Update(params IPlayerUnitEntity[] entities);
    }
}