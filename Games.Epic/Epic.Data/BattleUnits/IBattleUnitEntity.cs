using System;

namespace Epic.Data.BattleUnits
{
    public interface IBattleUnitEntity : IBattleUnitEntityFields
    {
        Guid Id { get; }
    }

    public interface IBattleUnitEntityFields
    {
        Guid BattleId { get; }
        Guid UserUnitId { get; }
        
        int Column { get; }
        int Row { get; }
        
        int PlayerIndex { get; }
    }

    internal class BattleUnitEntity : IBattleUnitEntity
    {
        public Guid Id { get; set; }
        public Guid BattleId { get; set; }
        public Guid UserUnitId { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public int PlayerIndex { get; set; }
    }
}