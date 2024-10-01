using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Epic.Data.UnitTypes
{
    public interface IUnitTypesRepository : IRepository
    {
        Task<IUnitTypeEntity> GetById(Guid id);
        Task<IReadOnlyCollection<IUnitTypeEntity>> FetchByIds(IEnumerable<Guid> ids);

        Task<IUnitTypeEntity> CreateUnitType(Guid id, IUnitTypeProperties properties);
    }
}