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
            BattleImgUrl = battleUnitObject.PlayerUnit.UnitType.BattleImgUrl;
            Speed = battleUnitObject.PlayerUnit.UnitType.Speed;
            Health = currentProps ? battleUnitObject.CurrentHealth : battleUnitObject.PlayerUnit.UnitType.Health;
            Attacks = battleUnitObject.PlayerUnit.UnitType.Attacks;
        }
    }
}
