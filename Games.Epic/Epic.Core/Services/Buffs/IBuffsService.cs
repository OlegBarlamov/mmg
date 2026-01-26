using System;
using System.Threading.Tasks;

namespace Epic.Core.Services.Buffs
{
    public interface IBuffsService
    {
        Task<IBuffObject> GetById(Guid id);
        Task<IBuffObject[]> GetByTargetBattleUnitId(Guid targetBattleUnitId);

        Task<IBuffObject> Create(Guid targetBattleUnitId, Guid buffTypeId, int durationRemaining);
        Task UpdateBatch(IBuffObject[] buffs);
        Task DeleteById(Guid id);
        Task DeleteByTargetBattleUnitId(Guid targetBattleUnitId);
    }
}

