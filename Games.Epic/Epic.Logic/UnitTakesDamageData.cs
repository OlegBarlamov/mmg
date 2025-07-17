using System;
using Epic.Core.Objects.BattleUnit;

namespace Epic.Logic
{
    public class UnitTakesDamageData
    {
        public int DamageTaken { get; set; }
        public int KilledCount { get; set; }
        public int RemainingHealth { get; set; }
        public int RemainingCount { get; set; }

        public static UnitTakesDamageData FromUnitAndTarget(IBattleUnitObject attacker, IBattleUnitObject target)
        {
            var damage = attacker.Damage * attacker.UserUnit.Count;
            var finalHealth = target.CurrentHealth - damage;
            var newCount = target.UserUnit.Count;
            if (finalHealth < 0)
            {
                var killedUnits = (int)Math.Truncate((double)finalHealth * (-1) / target.Health) + 1;
                finalHealth += killedUnits * target.Health;
                newCount = target.UserUnit.Count - killedUnits;

                if (newCount < 1)
                {
                    return new UnitTakesDamageData
                    {
                        DamageTaken =
                            target.UserUnit.Count * target.Health + target.CurrentHealth,
                        KilledCount = target.UserUnit.Count,
                        RemainingHealth = finalHealth,
                        RemainingCount = 0,
                    };
                }
            }

            return new UnitTakesDamageData
            {
                DamageTaken = damage,
                KilledCount = target.UserUnit.Count - newCount,
                RemainingHealth = finalHealth,
                RemainingCount = newCount,
            };
        }
    }
}