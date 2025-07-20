using System;
using Epic.Core.Services.Battles;

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
            var damage = attacker.Damage * attacker.PlayerUnit.Count;
            var finalHealth = target.CurrentHealth - damage;
            var newCount = target.PlayerUnit.Count;
            if (finalHealth < 0)
            {
                var killedUnits = (int)Math.Truncate((double)finalHealth * (-1) / target.Health) + 1;
                finalHealth += killedUnits * target.Health;
                newCount = target.PlayerUnit.Count - killedUnits;

                if (newCount < 1)
                {
                    return new UnitTakesDamageData
                    {
                        DamageTaken =
                            target.PlayerUnit.Count * target.Health + target.CurrentHealth,
                        KilledCount = target.PlayerUnit.Count,
                        RemainingHealth = finalHealth,
                        RemainingCount = 0,
                    };
                }
            }

            return new UnitTakesDamageData
            {
                DamageTaken = damage,
                KilledCount = target.PlayerUnit.Count - newCount,
                RemainingHealth = finalHealth,
                RemainingCount = newCount,
            };
        }
    }
}