using System;
using System.Threading.Tasks;

namespace Epic.Data.BattleUnits
{
    public interface IBattleUnitsRepository
    {
        Task<IBattleUnitEntity[]> GetByBattleId(Guid battleId);
    }
}