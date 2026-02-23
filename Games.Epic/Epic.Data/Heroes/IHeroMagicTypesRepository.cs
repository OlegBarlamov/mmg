using System;
using System.Threading.Tasks;

namespace Epic.Data.Heroes
{
    public interface IHeroMagicTypesRepository : IRepository
    {
        Task<Guid[]> GetMagicTypeIdsByHeroId(Guid heroId);
        Task Add(Guid heroId, Guid magicTypeId);
    }
}
