using System;
using System.Threading.Tasks;

namespace Epic.Data.MagicType
{
    public interface IMagicTypesRepository : IRepository
    {
        Task<IMagicTypeEntity> GetById(Guid id);
        Task<IMagicTypeEntity> GetByKey(string key);
        Task<IMagicTypeEntity[]> GetAll();

        Task<IMagicTypeEntity> Create(Guid id, IMagicTypeFields fields);
        Task<IMagicTypeEntity[]> CreateBatch(IMagicTypeFields[] fields);
        Task UpdateBatch(IMagicTypeEntity[] entities);
    }
}
