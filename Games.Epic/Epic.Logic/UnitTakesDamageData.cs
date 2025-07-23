using System;
using Epic.Core.Services.Battles;
using Epic.Data.UnitTypes.Subtypes;

namespace Epic.Logic
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
            if (counterAttack)
                throw new NotImplementedException();
            
            var randomDamage = random.Next(attackFunctionType.MinDamage, attackFunctionType.MaxDamage + 1);
            var damage = randomDamage * attacker.GlobalUnit.Count;
            if (attackFunctionType.RangePenalty)
            {
                damage = ApplyRangePenalty(
                    damage,
                    distanceToTarget,
                    attackFunctionType.AttackMinRange,
                    attackFunctionType.AttackMaxRange,
                    attackFunctionType.RangePenaltyZonesCount);
            }

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