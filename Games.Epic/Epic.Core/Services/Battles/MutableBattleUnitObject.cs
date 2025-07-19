using System;
using Epic.Data.BattleUnits;

namespace Epic.Core.Objects.BattleUnit
{
    public class MutableBattleUnitObject : IBattleUnitObject
    {
        public Guid Id { get; set; }
        public Guid BattleId { get; set; }
        public Guid UserUnitId { get; set; }
        public MutablePlayerUnitObject PlayerUnit { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public int PlayerIndex { get; set; }
        public int CurrentHealth { get; set; }
        public int Speed { get; set; }
        public int AttackMaxRange { get; set; }
        public int AttackMinRange { get; set; }
        public int Damage { get; set; }
        public int Health { get; set; }

        IPlayerUnitObject IBattleUnitObject.PlayerUnit => PlayerUnit;

        private MutableBattleUnitObject()
        {
        }
        
        public static MutableBattleUnitObject FromEntity(IBattleUnitEntity entity)
        {
            return new MutableBattleUnitObject
            {
                Id = entity.Id,
                BattleId = entity.BattleId,
                UserUnitId = entity.PlayerUnitId,
                PlayerUnit = null,
                Column = entity.Column,
                Row = entity.Row,
                PlayerIndex = entity.PlayerIndex,
                CurrentHealth = entity.CurrentHealth,
                Speed = entity.Speed,
                AttackMaxRange = entity.AttackMaxRange,
                AttackMinRange = entity.AttackMinRange,
                Damage = entity.Damage,
                Health = entity.Health,
            };
        }
        
        public static MutableBattleUnitObject CopyFrom(IBattleUnitObject x)
        {
            return new MutableBattleUnitObject
            {
                Id = x.Id,
                BattleId = x.BattleId,
                UserUnitId = x.PlayerUnit.Id,
                PlayerUnit = MutablePlayerUnitObject.CopyFrom(x.PlayerUnit),
                Column = x.Column,
                Row = x.Row,
                PlayerIndex = x.PlayerIndex,
                CurrentHealth = x.CurrentHealth,
                Speed = x.Speed,
                AttackMaxRange = x.AttackMaxRange,
                AttackMinRange = x.AttackMinRange,
                Damage = x.Damage,
                Health = x.Health,
            };
        }
    }
}