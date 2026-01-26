using System;
using System.Threading.Tasks;

namespace Epic.Data.BuffType
{
    public interface IBuffTypesRepository : IRepository
    {
        Task<IBuffTypeEntity> GetById(Guid id);
        Task<IBuffTypeEntity> GetByKey(string key);
        Task<IBuffTypeEntity[]> GetAll();

        Task<IBuffTypeEntity> Create(Guid id, IBuffFields fields);
        Task<IBuffTypeEntity[]> CreateBatch(IBuffFields[] fields);
        Task UpdateBatch(IBuffTypeEntity[] entities);
    }
}

