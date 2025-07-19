using System;
using Epic.Data.UnitTypes;

namespace Epic.Data.BattleUnits
{
    public interface IBattleUnitEntity : IBattleUnitEntityFields
    {
        Guid Id { get; }
    }

    public interface IBattleUnitEntityFields : IUnitProps
    {
        Guid BattleId { get; }
        Guid PlayerUnitId { get; }
        
        int Column { get; }
        int Row { get; }
        
        int PlayerIndex { get; }
        
        int CurrentHealth { get; }
    }

    internal class BattleUnitEntity : IBattleUnitEntity
    {
        public Guid Id { get; set; }
        public Guid BattleId { get; set; }
        public Guid PlayerUnitId { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public int PlayerIndex { get; set; }
        
        public int CurrentHealth { get; set; }
        public int Speed { get; set; }
        public int AttackMaxRange { get; set; }
        public int AttackMinRange { get; set; }
        public int Damage { get; set; }
        public int Health { get; set; }

        public BattleUnitEntity(IBattleUnitEntityFields fields)
        {
            UpdateFrom(fields);
        }

        public void UpdateFrom(IBattleUnitEntityFields fields)
        {
            BattleId = fields.BattleId;
            Column = fields.Column;
            Row = fields.Row;
            PlayerIndex = fields.PlayerIndex;
            PlayerUnitId = fields.PlayerUnitId;
            CurrentHealth = fields.CurrentHealth;
            Speed = fields.Speed;
            AttackMaxRange = fields.AttackMaxRange;
            AttackMinRange = fields.AttackMinRange;
            Damage = fields.Damage;
            Health = fields.Health;
        }
    }
}