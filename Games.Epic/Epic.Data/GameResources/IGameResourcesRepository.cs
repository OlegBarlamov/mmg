using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Epic.Data.GameResources
{
    public interface IGameResourcesRepository : IRepository
    {
        Guid GoldResourceId { get; }
        Task<IGameResourceEntity> GetById(Guid id);
        Task<IReadOnlyList<IGameResourceEntity>> GetByIds(IReadOnlyList<Guid> ids);
        Task<IGameResourceEntity> Create(string name, string iconUrl, int price);
        Task<IReadOnlyList<ResourceAmount>> GetAllResourcesByPlayer(Guid playerId);
        Task<ResourceAmount> GetResourceByPlayer(Guid resourceId, Guid playerId);
        
        Task<ResourceAmount> GiveResource(Guid resourceId, Guid playerId, int delta);
        Task<IReadOnlyDictionary<Guid, int>> GetResourcesAmount(Guid playerId);
        
        Task<bool> PayIfEnough(Price price, Guid playerId);
        
        Task Pay(Price price, Guid playerId);
        Task GiveResources(ResourceAmount[] resourceAmounts, Guid playerId);
    }
}