using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Epic.Data.Heroes
{
    public interface IHeroEntitiesRepository : IRepository
    {
        Task<IHeroEntity> GetById(Guid id);
        Task<IHeroEntity> Create(Guid id, IHeroEntityFields fields);
        Task<Guid> GetPlayerByHeroId(Guid heroId);
        Task<IHeroEntity> CreateForPlayer(Guid id, IHeroEntityFields fields, Guid playerId);
        Task GiveToPlayer(Guid heroId, Guid playerId);
        Task<IReadOnlyCollection<IHeroEntity>> GetByPlayerId(Guid playerId);
    }
}