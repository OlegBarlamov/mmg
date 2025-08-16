using System;
using Epic.Core.Services.Units;
using Epic.Data.BattleUnits;
using Epic.Data.Heroes;

namespace Epic.Core.Services.Battles
{
    public partial class DefaultBattleUnitsService
    {
        internal class BattleUnitEntityFields : IBattleUnitEntityFields
        {
            public Guid BattleId { get; set; }
            public Guid GlobalUnitId { get; set; }
            public int Column { get; set; }
            public int Row { get; set; }
            public int PlayerIndex { get; set; }
            public int CurrentHealth { get; set; }
            public int InitialCount { get; set; }
            public int CurrentCount { get; set; }
            public int CurrentAttack { get; set; }
            public int CurrentDefense { get; set; }
            public bool Waited { get; set; }


            protected BattleUnitEntityFields() {}
            
            public static BattleUnitEntityFields FromUserUnit(
                IGlobalUnitObject globalUnit, 
                Guid battleId, 
                InBattlePlayerNumber playerNumber,
                IHeroStats heroStats)
            {
                return new BattleUnitEntityFields
                {
                    BattleId = battleId,
                    GlobalUnitId = globalUnit.Id,
                    Column = -1,
                    Row = globalUnit.ContainerSlotIndex,
                    PlayerIndex = (int)playerNumber,
                    CurrentHealth = globalUnit.UnitType.Health,
                    InitialCount = globalUnit.Count,
                    CurrentCount = globalUnit.Count,
                    Waited = false,
                    CurrentAttack = heroStats.Attack,
                    CurrentDefense = heroStats.Defense,
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
                    GlobalUnitId = battleUnit.GlobalUnit.Id,
                    Column = battleUnit.Column,
                    Row = battleUnit.Row,
                    PlayerIndex = battleUnit.PlayerIndex,
                    CurrentHealth = battleUnit.CurrentHealth,
                    InitialCount = battleUnit.InitialCount,
                    CurrentCount = battleUnit.CurrentCount,
                    Waited = battleUnit.Waited,
                    CurrentAttack = battleUnit.CurrentAttack,
                    CurrentDefense = battleUnit.CurrentDefense,
                };
            }
        }
    }
}