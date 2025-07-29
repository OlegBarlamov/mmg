using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Epic.Data.BattleUnits
{
    public interface IBattleUnitsRepository : IRepository
    {
        Task<IBattleUnitEntity[]> GetByBattleId(Guid battleId);
        
        Task<IBattleUnitEntity[]> CreateBatch(IBattleUnitEntityFields[] data);

        Task Update(IBattleUnitEntity[] entities);

        Task<AttackFunctionStateEntity[]> GetAttacksData(Guid battleUnitId);
        Task UpdateAttacksData(IReadOnlyCollection<AttackFunctionStateEntity> attacksData);
        Task InsertAttacksData(IReadOnlyCollection<AttackFunctionStateEntity> attacksDataPerUnitId);
    }
}