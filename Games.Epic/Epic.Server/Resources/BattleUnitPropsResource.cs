using System.Collections.Generic;
using System.Linq;
using Epic.Core.Services.Buffs;
using Epic.Core.Services.Battles;
using Epic.Data.BattleUnits;
using Epic.Data.UnitTypes;
using Epic.Data.UnitTypes.Subtypes;

namespace Epic.Server.Resources
{
    public class BattleUnitPropsResource : IUnitProps
    {
        public string BattleImgUrl { get; }
        public int Speed { get; }
        public int Health { get; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public MovementType Movement { get; set; }
        public string MovementType { get; set; }
        public IReadOnlyList<IAttackFunctionType> Attacks { get; }
        public IReadOnlyList<AttackFunctionStateEntity> AttacksStates { get; }
        public bool Waited { get; }
        
        public IReadOnlyList<BattleUnitBuffResource> Buffs { get; }

        public BattleUnitPropsResource(IBattleUnitObject battleUnitObject, bool currentProps)
        {
            BattleImgUrl = battleUnitObject.GlobalUnit.UnitType.BattleImgUrl;
            Speed = battleUnitObject.GlobalUnit.UnitType.Speed;
            Waited = battleUnitObject.Waited;
            Health = currentProps ? battleUnitObject.CurrentHealth : battleUnitObject.GlobalUnit.UnitType.Health;
            Attacks = battleUnitObject.GlobalUnit.UnitType.Attacks;
            Attack = currentProps ? battleUnitObject.CurrentAttack : battleUnitObject.GlobalUnit.UnitType.Attack;
            Defense = currentProps ? battleUnitObject.CurrentDefense : battleUnitObject.GlobalUnit.UnitType.Defense;
            Movement = battleUnitObject.GlobalUnit.UnitType.Movement;
            MovementType = battleUnitObject.GlobalUnit.UnitType.Movement.ToString();
            if (currentProps)
                AttacksStates = battleUnitObject.AttackFunctionsData;

            Buffs = (battleUnitObject.Buffs ?? System.Array.Empty<IBuffObject>())
                .Select(x => new BattleUnitBuffResource(x))
                .ToList();
        }
    }
}
