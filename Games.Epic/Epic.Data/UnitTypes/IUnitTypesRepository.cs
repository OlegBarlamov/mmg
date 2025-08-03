using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Epic.Data.UnitTypes
{
    public interface IUnitTypesRepository : IRepository
    {
        Task<IUnitTypeEntity> GetById(Guid id);
        Task<IUnitTypeEntity> GetByName(string name);
        Task<IReadOnlyCollection<IUnitTypeEntity>> FetchByIds(IEnumerable<Guid> ids);

        Task<IUnitTypeEntity> CreateUnitType(Guid id, UnitTypeProperties properties);
        Task<IUnitTypeEntity[]> CreateBatch(IEnumerable<UnitTypeProperties> properties);
        Task<IUnitTypeEntity[]> GetAll();
    }
}