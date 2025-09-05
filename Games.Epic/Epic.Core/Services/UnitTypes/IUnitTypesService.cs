using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epic.Data.GameResources;

namespace Epic.Core.Services.UnitTypes
{
    public interface IUnitTypesService
    {
        Task<IUnitTypeObject> GetUnitTypeByIdAsync(Guid id);
        Task<IReadOnlyList<IUnitTypeObject>> GetUnitTypesByIdsAsync(IReadOnlyCollection<Guid> ids);
        Task<Price> GetPrice(IUnitTypeObject unitType);
        Task<Price> GetPriceForUpgrade(IUnitTypeObject originalUnitType, IUnitTypeObject targetUnitType);
        Task<Price[]> GetPrices(IReadOnlyList<IUnitTypeObject> unitTypes);
    }
}