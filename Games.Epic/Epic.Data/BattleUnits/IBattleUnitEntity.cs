using System;

namespace Epic.Data.BattleUnits
{
    public interface IBattleUnitEntity
    {
        Guid Id { get; }
        Guid BattleId { get; }
        Guid UserUnitId { get; }
        
        int Column { get; }
        int Row { get; }
        
        int PlayerIndex { get; }
    }
}