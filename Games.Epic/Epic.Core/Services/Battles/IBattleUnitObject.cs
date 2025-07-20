using System;
using Epic.Core.Objects;
using Epic.Core.Services.Units;
using Epic.Data.BattleUnits;
using Epic.Data.UnitTypes;

namespace Epic.Core.Services.Battles
{
    public interface IBattleUnitObject : IGameObject<IBattleUnitEntity>, IUnitProps
    {
        Guid Id { get; }
        Guid BattleId { get; }
        IPlayerUnitObject PlayerUnit { get; }
        
        int Column { get; }
        int Row { get; }
        int PlayerIndex { get; }
        int CurrentHealth { get; }
    }
}