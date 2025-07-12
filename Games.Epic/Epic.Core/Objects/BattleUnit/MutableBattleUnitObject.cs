using System;

namespace Epic.Core.Objects.BattleUnit
{
    public class MutableBattleUnitObject : IBattleUnitObject
    {
        public Guid Id { get; set; }
        public Guid BattleId { get; set; }
        public Guid UserUnitId { get; set; }
        public MutableUserUnitObject UserUnit { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public int PlayerIndex { get; set; }

        IUserUnitObject IBattleUnitObject.UserUnit => UserUnit;

        public static MutableBattleUnitObject CopyFrom(IBattleUnitObject x)
        {
            return new MutableBattleUnitObject
            {
                Id = x.Id,
                BattleId = x.BattleId,
                UserUnitId = x.UserUnit.Id,
                UserUnit = new MutableUserUnitObject
                {
                    Id = x.UserUnit.Id,
                    UnitType = x.UserUnit.UnitType,
                    UnitTypeId = x.UserUnit.UnitType.Id,
                    Count = x.UserUnit.Count,
                    UserId = x.UserUnit.UserId,
                    IsAlive = x.UserUnit.IsAlive,
                },
                Column = x.Column,
                Row = x.Row,
                PlayerIndex = x.PlayerIndex,
            };
        }
    }
}