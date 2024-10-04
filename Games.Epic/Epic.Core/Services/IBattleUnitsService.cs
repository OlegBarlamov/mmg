using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epic.Core.Objects;
using Epic.Core.Objects.BattleUnit;

namespace Epic.Core
{
    public interface IBattleUnitsService
    {
        Task<IReadOnlyCollection<IBattleUnitObject>> GetBattleUnits(Guid battleId);
        Task<IReadOnlyCollection<IBattleUnitObject>> CreateUnitsFromBattleDefinition(IBattleDefinitionObject battleDefinition, Guid battleId);
        Task<IReadOnlyCollection<IBattleUnitObject>> CreateUnitsFromUserUnits(IReadOnlyCollection<IUserUnitObject> userUnits, int playerIndex, Guid battleId);
        Task<IReadOnlyCollection<IBattleUnitObject>> UpdateUnits(IReadOnlyCollection<IBattleUnitObject> battleUnit);
    }
}