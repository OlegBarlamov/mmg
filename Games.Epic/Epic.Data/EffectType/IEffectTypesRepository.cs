using System;
using System.Threading.Tasks;

namespace Epic.Data.EffectType
{
    public interface IEffectTypesRepository : IRepository
    {
        Task<IEffectTypeEntity> GetById(Guid id);
        Task<IEffectTypeEntity> GetByKey(string key);
        Task<IEffectTypeEntity[]> GetAll();

        Task<IEffectTypeEntity> Create(Guid id, IEffectTypeFields fields);
        Task<IEffectTypeEntity[]> CreateBatch(IEffectTypeFields[] fields);
        Task UpdateBatch(IEffectTypeEntity[] entities);
    }
}
