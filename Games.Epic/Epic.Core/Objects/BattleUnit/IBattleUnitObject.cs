using System;

namespace Epic.Core.Objects.BattleUnit
{
    public interface IBattleUnitObject
    {
        Guid Id { get; }
        Guid BattleId { get; }
        IUserUnitObject UserUnit { get; }
        
        int Column { get; }
        int Row { get; }
        
        int PlayerIndex { get; }
    }
}