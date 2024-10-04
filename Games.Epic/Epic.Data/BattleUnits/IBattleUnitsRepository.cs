using System;
using System.Threading.Tasks;

namespace Epic.Data.BattleUnits
{
    public interface IBattleUnitsRepository : IRepository
    {
        Task<IBattleUnitEntity[]> GetByBattleId(Guid battleId);
        
        Task<IBattleUnitEntity[]> CreateBatch(IBattleUnitEntityFields[] data);
    }
}