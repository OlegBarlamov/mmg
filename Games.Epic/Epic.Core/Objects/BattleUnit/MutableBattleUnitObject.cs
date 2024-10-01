using System;

namespace Epic.Core.Objects.BattleUnit
{
    public class MutableBattleUnitObject : IBattleUnitObject
    {
        public Guid Id { get; set; }
        public Guid BattleId { get; set; }
        public Guid UserUnitId { get; set; }
        public IUserUnitObject UserUnit { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public int PlayerIndex { get; set; }
    }
}