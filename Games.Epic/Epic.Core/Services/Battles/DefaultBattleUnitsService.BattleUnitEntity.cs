using System;
using Epic.Core.Objects;
using Epic.Core.Services.Units;
using Epic.Data.BattleUnits;
using Epic.Data.UnitTypes;

namespace Epic.Core.Services.Battles
{
    public partial class DefaultBattleUnitsService
    {
        internal class BattleUnitEntity : IBattleUnitEntity
        {
            private Guid? Id { get; set; }
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
            
            Guid IBattleUnitEntity.Id => Id ?? throw new InvalidOperationException("Battle Unit Entity has not been set for updating an instance in the repository");
            
            private BattleUnitEntity(IUnitProps props)
            {
                Health = props.Health;
                Speed = props.Speed;
                AttackMaxRange = props.AttackMaxRange;
                AttackMinRange = props.AttackMinRange;
                Damage = props.Damage;
            }
            
            public static BattleUnitEntity FromUserUnit(IPlayerUnitObject playerUnit, Guid battleId, InBattlePlayerNumber playerNumber, Guid? id = null)
            {
                return new BattleUnitEntity(playerUnit.UnitType)
                {
                    Id = id,
                    BattleId = battleId,
                    PlayerUnitId = playerUnit.Id,
                    Column = -1,
                    Row = -1,
                    PlayerIndex = (int)playerNumber,
                    CurrentHealth = playerUnit.UnitType.Health,
                };
            }

            public static BattleUnitEntity FromBattleUnitObject(IBattleUnitObject battleUnit)
            {
                return new BattleUnitEntity(battleUnit)
                {
                    Id = battleUnit.Id,
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