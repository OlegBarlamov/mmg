using System.Collections.Generic;
using System.Linq;
using Epic.Core.Services.Buffs;
using Epic.Core.Services.BuffTypes;
using Epic.Core.Services.Battles;
using Epic.Data.BattleUnits;
using Epic.Data.UnitTypes.Subtypes;
using static Epic.Logic.Battle.BattleUnitBuffExtensions;

namespace Epic.Server.Resources
{
    public class BattleUnitPropsResource
    {
        public string BattleImgUrl { get; }
        public int Speed { get; }
        public int Health { get; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public MovementType Movement { get; set; }
        public string MovementType { get; set; }
        public IReadOnlyList<AttackTypeResource> Attacks { get; }
        public IReadOnlyList<AttackFunctionStateEntity> AttacksStates { get; }
        public bool Waited { get; }
        
        public IReadOnlyList<BattleUnitBuffResource> Buffs { get; }

        public BattleUnitPropsResource(IBattleUnitObject battleUnitObject, bool currentProps, IBuffTypesRegistry buffTypesRegistry)
        {
            BattleImgUrl = battleUnitObject.GlobalUnit.UnitType.BattleImgUrl;
            Speed = battleUnitObject.GlobalUnit.UnitType.Speed;
            Waited = battleUnitObject.Waited;
            Health = currentProps ? battleUnitObject.CurrentHealth : battleUnitObject.GetEffectiveMaxHealth();
            Attacks = battleUnitObject.GlobalUnit.UnitType.Attacks
                .Select(a => new AttackTypeResource(a, buffTypesRegistry))
                .ToList();
            Attack = currentProps ? battleUnitObject.GetEffectiveAttack() : battleUnitObject.GlobalUnit.UnitType.Attack;
            Defense = currentProps ? battleUnitObject.GetEffectiveDefense() : battleUnitObject.GlobalUnit.UnitType.Defense;
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
