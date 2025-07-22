using System;
using Epic.Core.Services.Units;
using Epic.Data.BattleUnits;

namespace Epic.Core.Services.Battles
{
    public class MutableBattleUnitObject : IBattleUnitObject
    {
        public Guid Id { get; }
        public Guid BattleId { get; set; }
        public Guid PlayerUnitId { get; set; }
        public MutablePlayerUnitObject PlayerUnit { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public int PlayerIndex { get; set; }
        public int CurrentHealth { get; set; }
        
        IPlayerUnitObject IBattleUnitObject.PlayerUnit => PlayerUnit;

        private MutableBattleUnitObject(Guid id)
        {
            Id = id;
        }
        
        public static MutableBattleUnitObject FromEntity(IBattleUnitEntity entity)
        {
            return new MutableBattleUnitObject(entity.Id)
            {
                BattleId = entity.BattleId,
                PlayerUnitId = entity.PlayerUnitId,
                Column = entity.Column,
                Row = entity.Row,
                PlayerIndex = entity.PlayerIndex,
                CurrentHealth = entity.CurrentHealth,
            };
        }
        
        public static MutableBattleUnitObject CopyFrom(IBattleUnitObject x)
        {
            var entity = x.ToEntity();
            var battleUnitObject = FromEntity(entity);
            battleUnitObject.PlayerUnit = MutablePlayerUnitObject.CopyFrom(x.PlayerUnit);
            return battleUnitObject;
        }

        public IBattleUnitEntity ToEntity()
        {
            return DefaultBattleUnitsService.BattleUnitEntity.FromBattleUnitObject(this);
        }
    }
}