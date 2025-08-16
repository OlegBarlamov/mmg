using System;
using System.Linq;
using Epic.Core.Services.Battles;
using Epic.Data.Heroes;
using Epic.Data.UnitTypes.Subtypes;

namespace Epic.Logic.Battle.Formulas
{
    public class UnitTakesDamageData
    {
        public int DamageTaken { get; set; }
        public int KilledCount { get; set; }
        public int RemainingHealth { get; set; }
        public int RemainingCount { get; set; }

        public static UnitTakesDamageData FromUnitAndTarget(
            IBattleUnitObject attacker,
            IBattleUnitObject target,
            IAttackFunctionType attackFunctionType,
            int distanceToTarget,
            bool counterAttack,
            Random random)
        {
            var dicesCounts = Math.Min(attacker.GlobalUnit.Count, 10);
            var randomSum = Enumerable.Range(0, dicesCounts)
                .Sum(x => random.Next(attackFunctionType.MinDamage, attackFunctionType.MaxDamage + 1));
            
            var damage = randomSum / dicesCounts * attacker.GlobalUnit.Count;
            
            // Apply Attack vs Defense modifier
            var attackDefenseDiff = attacker.CurrentAttack - target.CurrentDefense;
            var multiplier = 1.0;
            if (attackDefenseDiff > 0)
            {
                multiplier = 1 + Math.Min(attackDefenseDiff * 0.05, 3.0); // +5% per point, max +300%
            }
            else if (attackDefenseDiff < 0)
            {
                multiplier = 1 + (attackDefenseDiff * 0.025); // -2.5% per point, no strict cap
            }
            damage = (int)(damage * multiplier);
            
            if (attackFunctionType.RangePenalty)
            {
                damage = ApplyRangePenalty(
                    damage,
                    distanceToTarget,
                    attackFunctionType.AttackMinRange,
                    attackFunctionType.AttackMaxRange,
                    attackFunctionType.RangePenaltyZonesCount);
            }

            if (counterAttack)
                damage = (int)(damage * 0.01 * (100 - attackFunctionType.CounterattacksPenaltyPercentage));
            
            var finalHealth = target.CurrentHealth - damage;
            var newCount = target.GlobalUnit.Count;
            if (finalHealth < 0)
            {
                var killedUnits = (int)Math.Truncate((double)finalHealth * (-1) / target.GlobalUnit.UnitType.Health) + 1;
                finalHealth += killedUnits * target.GlobalUnit.UnitType.Health;
                newCount = target.GlobalUnit.Count - killedUnits;

                if (newCount < 1)
                {
                    return new UnitTakesDamageData
                    {
                        DamageTaken =
                            target.GlobalUnit.Count * target.GlobalUnit.UnitType.Health + target.CurrentHealth,
                        KilledCount = target.GlobalUnit.Count,
                        RemainingHealth = finalHealth,
                        RemainingCount = 0,
                    };
                }
            }

            return new UnitTakesDamageData
            {
                DamageTaken = damage,
                KilledCount = target.GlobalUnit.Count - newCount,
                RemainingHealth = finalHealth,
                RemainingCount = newCount,
            };
        }

        private static int ApplyRangePenalty(int damage, int range, int attackMinRange, int attackMaxRange, int rangePenaltyZonesCount)
        {
            double distanceRatio = (double)(range - attackMinRange) / (attackMaxRange - attackMinRange);
            double finalDamage = damage;

            if (rangePenaltyZonesCount > 0)
            {
                double zoneStep = 1.0 / (rangePenaltyZonesCount + 1);
                for (int i = 1; i <= rangePenaltyZonesCount; i++)
                {
                    if (distanceRatio >= zoneStep * i)
                    {
                        finalDamage *= 0.5;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return (int)Math.Round(finalDamage);
        }
    }
}