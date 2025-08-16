using System;
using System.Collections.Generic;
using Epic.Core.Services.Units;
using Epic.Data.BattleUnits;

namespace Epic.Core.Services.Battles
{
    public class MutableBattleUnitObject : IBattleUnitObject
    {
        public Guid Id { get; }
        public Guid BattleId { get; set; }
        public Guid PlayerUnitId { get; set; }
        public MutableGlobalUnitObject GlobalUnit { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public int PlayerIndex { get; set; }
        public int CurrentHealth { get; set; }
        public int InitialCount { get; set; }
        public int CurrentCount { get; set; }
        public bool Waited { get; set; }
        public int CurrentAttack { get; set; }
        public int CurrentDefense { get; set; }

        public IReadOnlyList<AttackFunctionStateEntity> AttackFunctionsData { get; set; }

        IGlobalUnitObject IBattleUnitObject.GlobalUnit => GlobalUnit;

        private MutableBattleUnitObject(Guid id)
        {
            Id = id;
        }
        
        public static MutableBattleUnitObject FromEntity(IBattleUnitEntity entity)
        {
            return new MutableBattleUnitObject(entity.Id)
            {
                BattleId = entity.BattleId,
                PlayerUnitId = entity.GlobalUnitId,
                Column = entity.Column,
                Row = entity.Row,
                PlayerIndex = entity.PlayerIndex,
                CurrentHealth = entity.CurrentHealth,
                InitialCount = entity.InitialCount,
                CurrentCount = entity.CurrentCount,
                Waited = entity.Waited,
                CurrentAttack = entity.CurrentAttack,
                CurrentDefense = entity.CurrentDefense,
            };
        }
        
        public static MutableBattleUnitObject CopyFrom(IBattleUnitObject x)
        {
            var entity = x.ToEntity();
            var battleUnitObject = FromEntity(entity);
            battleUnitObject.GlobalUnit = MutableGlobalUnitObject.CopyFrom(x.GlobalUnit);
            battleUnitObject.AttackFunctionsData = x.AttackFunctionsData;
            return battleUnitObject;
        }

        public IBattleUnitEntity ToEntity()
        {
            return DefaultBattleUnitsService.BattleUnitEntity.FromBattleUnitObject(this);
        }
    }
}