using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epic.Core.Objects.BattleUnit;

namespace Epic.Core
{
    public interface IBattleUnitsService
    {
        Task<IReadOnlyCollection<IBattleUnitObject>> GetBattleUnits(Guid battleId); 
    }
}