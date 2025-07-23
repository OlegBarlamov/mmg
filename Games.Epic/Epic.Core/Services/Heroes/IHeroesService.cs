using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Epic.Core.Services.Heroes
{
    public interface IHeroesService
    {
        Task<IHeroObject> GetById(Guid id);
        Task<IHeroObject> CreateNew(string name, Guid playerId, bool setActive = false);
        Task<IReadOnlyList<IHeroObject>> GetByPlayerId(Guid playerId);
    }
}