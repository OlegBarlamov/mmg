using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epic.Core.Objects;
using Epic.Core.Services.BattleDefinitions;
using Epic.Core.Services.Units;
using Epic.Data.Heroes;

namespace Epic.Core.Services.Battles
{
    public interface IBattleUnitsService
    {
        Task<IReadOnlyCollection<IBattleUnitObject>> GetBattleUnits(Guid battleId);
        Task<IReadOnlyCollection<IBattleUnitObject>> CreateUnitsFromBattleDefinition(IBattleDefinitionObject battleDefinition, Guid battleId);
        Task<IReadOnlyCollection<IBattleUnitObject>> CreateBattleUnitsFromGlobalUnits(IReadOnlyCollection<IGlobalUnitObject> playerUnits, InBattlePlayerNumber playerNumber, Guid battleId, IHeroStats heroStats);
        Task UpdateUnits(IReadOnlyCollection<IBattleUnitObject> battleUnit);
        IReadOnlyCollection<IGlobalUnitObject> PickUnitsFitToBattleSize(IReadOnlyCollection<IGlobalUnitObject> units, ISize size);
    }
}
