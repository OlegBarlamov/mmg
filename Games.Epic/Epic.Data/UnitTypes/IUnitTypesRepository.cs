using System;
using System.Threading.Tasks;

namespace Epic.Data.UnitTypes
{
    public interface IUnitTypesRepository
    {
        Task<IUnitTypeEntity> GetById(Guid id);
    }
}