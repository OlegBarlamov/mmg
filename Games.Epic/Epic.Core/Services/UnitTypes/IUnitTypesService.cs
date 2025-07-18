using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Epic.Core
{
    public interface IUnitTypesService
    {
        Task<IUnitTypeObject> GetUnitTypeByIdAsync(Guid id);
        Task<IReadOnlyCollection<IUnitTypeObject>> GetUnitTypesByIdsAsync(IReadOnlyCollection<Guid> ids);
    }
}