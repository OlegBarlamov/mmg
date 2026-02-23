using System;
using System.Threading.Tasks;
using Epic.Data.Buff;

namespace Epic.Core.Services.Buffs
{
    public interface IBuffsService
    {
        Task<IBuffObject> GetById(Guid id);
        Task<IBuffObject[]> GetByTargetBattleUnitId(Guid targetBattleUnitId);

        Task<IBuffObject> Create(Guid targetBattleUnitId, Guid buffTypeId, BuffExpressionsVariables variables);
        /// <summary>Create a buff with pre-evaluated effective values (e.g. when applying magic).</summary>
        Task<IBuffObject> Create(Guid targetBattleUnitId, Guid buffTypeId, IBuffEffectiveValues effectiveValues);
        Task UpdateBatch(IBuffObject[] buffs);
        Task DeleteById(Guid id);
        Task DeleteByTargetBattleUnitId(Guid targetBattleUnitId);
    }
}

