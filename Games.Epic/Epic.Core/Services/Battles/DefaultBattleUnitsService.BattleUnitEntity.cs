using System;
using Epic.Core.Services.Units;
using Epic.Data.BattleUnits;

namespace Epic.Core.Services.Battles
{
    public partial class DefaultBattleUnitsService
    {
        internal class BattleUnitEntityFields : IBattleUnitEntityFields
        {
            public Guid BattleId { get; set; }
            public Guid PlayerUnitId { get; set; }
            public int Column { get; set; }
            public int Row { get; set; }
            public int PlayerIndex { get; set; }
            public int CurrentHealth { get; set; }
            
            protected BattleUnitEntityFields() {}
            
            public static BattleUnitEntityFields FromUserUnit(IPlayerUnitObject playerUnit, Guid battleId, InBattlePlayerNumber playerNumber)
            {
                return new BattleUnitEntityFields
                {
                    BattleId = battleId,
                    PlayerUnitId = playerUnit.Id,
                    Column = -1,
                    Row = playerUnit.ContainerSlotIndex,
                    PlayerIndex = (int)playerNumber,
                    CurrentHealth = playerUnit.UnitType.Health,
                };
            }
        }
        
        internal class BattleUnitEntity : BattleUnitEntityFields, IBattleUnitEntity
        {
            public Guid Id { get; }
            
            private BattleUnitEntity(Guid id)
            {
                Id = id;
            }

            public static BattleUnitEntity FromBattleUnitObject(IBattleUnitObject battleUnit)
            {
                return new BattleUnitEntity(battleUnit.Id)
                {
                    BattleId = battleUnit.BattleId,
                    PlayerUnitId = battleUnit.PlayerUnit.Id,
                    Column = battleUnit.Column,
                    Row = battleUnit.Row,
                    PlayerIndex = battleUnit.PlayerIndex,
                    CurrentHealth = battleUnit.CurrentHealth,
                };
            }
        }
    }
}