using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Epic.Core.Services.UnitTypes
{
    public interface IUnitTypesService
    {
        Task<IUnitTypeObject> GetUnitTypeByIdAsync(Guid id);
        Task<IReadOnlyCollection<IUnitTypeObject>> GetUnitTypesByIdsAsync(IReadOnlyCollection<Guid> ids);
    }
}