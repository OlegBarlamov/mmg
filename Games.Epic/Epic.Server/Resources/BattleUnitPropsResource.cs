using System.Collections.Generic;
using Epic.Core.Services.Battles;
using Epic.Data.UnitTypes;
using Epic.Data.UnitTypes.Subtypes;

namespace Epic.Server.Resources
{
    public class BattleUnitPropsResource : IUnitProps
    {
        public string BattleImgUrl { get; }
        public int Speed { get; set; }
        public int Health { get; set; }
        public IReadOnlyList<IAttackFunctionType> Attacks { get; set; }

        public BattleUnitPropsResource(IBattleUnitObject battleUnitObject, bool currentProps)
        {
            BattleImgUrl = battleUnitObject.GlobalUnit.UnitType.BattleImgUrl;
            Speed = battleUnitObject.GlobalUnit.UnitType.Speed;
            Health = currentProps ? battleUnitObject.CurrentHealth : battleUnitObject.GlobalUnit.UnitType.Health;
            Attacks = battleUnitObject.GlobalUnit.UnitType.Attacks;
        }
    }
}
