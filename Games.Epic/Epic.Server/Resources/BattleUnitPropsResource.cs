using Epic.Core.Objects.BattleUnit;

namespace Epic.Server.Resources
{
    public class BattleUnitPropsResource
    {
        public string BattleImgUrl { get; }
        public int Speed { get; }
        public int AttackMaxRange { get; }
        public int AttackMinRange { get; }
        public int Damage { get; }
        public int Health { get; }

        public BattleUnitPropsResource(IBattleUnitObject battleUnitObject, bool currentProps)
        {
            BattleImgUrl = battleUnitObject.PlayerUnit.UnitType.BattleImgUrl;
            Speed = battleUnitObject.Speed;
            AttackMinRange = battleUnitObject.AttackMinRange;
            AttackMaxRange = battleUnitObject.AttackMaxRange;
            Damage = battleUnitObject.Damage;
            Health = currentProps ? battleUnitObject.CurrentHealth : battleUnitObject.Health;
        }
    }
}