using System;
using Epic.Data.UnitTypes;

namespace Epic.Core.Objects.BattleUnit
{
    public interface IBattleUnitObject : IUnitProps
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