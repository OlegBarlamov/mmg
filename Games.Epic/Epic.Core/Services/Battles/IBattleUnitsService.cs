using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epic.Core.Objects;
using Epic.Core.Services.BattleDefinitions;
using Epic.Core.Services.Units;

namespace Epic.Core.Services.Battles
{
    public interface IBattleUnitsService
    {
        Task<IReadOnlyCollection<IBattleUnitObject>> GetBattleUnits(Guid battleId);
        Task<IReadOnlyCollection<IBattleUnitObject>> CreateUnitsFromBattleDefinition(IBattleDefinitionObject battleDefinition, Guid battleId);
        Task<IReadOnlyCollection<IBattleUnitObject>> CreateUnitsFromPlayerUnits(IReadOnlyCollection<IPlayerUnitObject> playerUnits, InBattlePlayerNumber playerNumber, Guid battleId);
        Task UpdateUnits(IReadOnlyCollection<IBattleUnitObject> battleUnit);
    }
}
