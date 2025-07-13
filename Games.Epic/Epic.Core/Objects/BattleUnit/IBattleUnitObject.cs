using System;
using Epic.Data.UnitTypes;

namespace Epic.Core.Objects.BattleUnit
{
    public interface IBattleUnitObject : IUnitProps
    {
        Guid Id { get; }
        Guid BattleId { get; }
        IUserUnitObject UserUnit { get; }
        
        int Column { get; }
        int Row { get; }
        int PlayerIndex { get; }
        int CurrentHealth { get; }
    }
}