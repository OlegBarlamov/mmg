using System;

namespace Epic.Data.BattleUnits
{
    public interface IBattleUnitEntity : IBattleUnitEntityFields
    {
        Guid Id { get; }
    }

    public interface IBattleUnitState
    {
        bool Waited { get; }
    }

    public interface IBattleUnitEntityFields : IBattleUnitState
    {
        Guid BattleId { get; }
        Guid PlayerUnitId { get; }
        
        int Column { get; }
        int Row { get; }
        
        int PlayerIndex { get; }
        
        int CurrentHealth { get; }
        
        int InitialCount { get; }
        
        int CurrentCount { get; }
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
        public int InitialCount { get; set; }
        public int CurrentCount { get; set; }
        
        public bool Waited { get; set; }


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
            InitialCount = fields.InitialCount;
            CurrentCount = fields.CurrentCount;
            Waited = fields.Waited;
        }
    }
}