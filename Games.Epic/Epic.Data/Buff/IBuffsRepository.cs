using System;
using System.Threading.Tasks;

namespace Epic.Data.Buff
{
    public interface IBuffsRepository : IRepository
    {
        Task<IBuffEntity> GetById(Guid id);
        Task<IBuffEntity[]> GetByTargetBattleUnitId(Guid targetBattleUnitId);
        Task<IBuffEntity[]> GetAll();

        Task<IBuffEntity> Create(Guid id, IBuffEntityFields fields);
        Task<IBuffEntity[]> CreateBatch(IBuffEntityFields[] fields);
        Task UpdateBatch(IBuffEntity[] entities);

        Task DeleteById(Guid id);
        Task DeleteByTargetBattleUnitId(Guid targetBattleUnitId);
    }
}

