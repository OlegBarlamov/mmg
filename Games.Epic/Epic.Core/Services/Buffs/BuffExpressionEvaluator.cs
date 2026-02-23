using Epic.Data.Buff;
using Epic.Data.BuffType;
using Epic.Core.Utils;

namespace Epic.Core.Services.Buffs
{
    public static class BuffExpressionEvaluator
    {
        public static BuffEffectiveValues Evaluate(
            IBuffFields buffType,
            BuffExpressionsVariables variables)
        {
            var interpreter = ExpressionEvaluator.CreateInterpreter(variables.ToDictionary());
            return new BuffEffectiveValues
            {
                HealthBonus = ExpressionEvaluator.EvalInt(buffType.HealthBonusExpression, interpreter),
                AttackBonus = ExpressionEvaluator.EvalInt(buffType.AttackBonusExpression, interpreter),
                DefenseBonus = ExpressionEvaluator.EvalInt(buffType.DefenseBonusExpression, interpreter),
                SpeedBonus = ExpressionEvaluator.EvalInt(buffType.SpeedBonusExpression, interpreter),
                MinDamageBonus = ExpressionEvaluator.EvalInt(buffType.MinDamageBonusExpression, interpreter),
                MaxDamageBonus = ExpressionEvaluator.EvalInt(buffType.MaxDamageBonusExpression, interpreter),
                HealthBonusPercentage = ExpressionEvaluator.EvalInt(buffType.HealthBonusPercentageExpression, interpreter),
                AttackBonusPercentage = ExpressionEvaluator.EvalInt(buffType.AttackBonusPercentageExpression, interpreter),
                DefenseBonusPercentage = ExpressionEvaluator.EvalInt(buffType.DefenseBonusPercentageExpression, interpreter),
                SpeedBonusPercentage = ExpressionEvaluator.EvalInt(buffType.SpeedBonusPercentageExpression, interpreter),
                MinDamageBonusPercentage = ExpressionEvaluator.EvalInt(buffType.MinDamageBonusPercentageExpression, interpreter),
                MaxDamageBonusPercentage = ExpressionEvaluator.EvalInt(buffType.MaxDamageBonusPercentageExpression, interpreter),
                Paralyzed = buffType.Paralyzed,
                Stunned = buffType.Stunned,
                VampirePercentage = ExpressionEvaluator.EvalInt(buffType.VampirePercentageExpression, interpreter),
                VampireCanResurrect = buffType.VampireCanResurrect,
                DeclinesWhenTakesDamage = buffType.DeclinesWhenTakesDamage,
                Heals = ExpressionEvaluator.EvalInt(buffType.HealsExpression, interpreter),
                HealsPercentage = ExpressionEvaluator.EvalInt(buffType.HealsPercentageExpression, interpreter),
                HealCanResurrect = buffType.HealCanResurrect,
                TakesDamageMin = ExpressionEvaluator.EvalInt(buffType.TakesDamageMinExpression, interpreter),
                TakesDamageMax = ExpressionEvaluator.EvalInt(buffType.TakesDamageMaxExpression, interpreter),
                DamageReturnPercentage = ExpressionEvaluator.EvalInt(buffType.DamageReturnPercentageExpression, interpreter),
                DamageReturnMaxRange = ExpressionEvaluator.EvalInt(buffType.DamageReturnMaxRangeExpression, interpreter),
                Permanent = buffType.Permanent,
                Duration = ExpressionEvaluator.EvalInt(buffType.DurationExpression, interpreter),
            };
        }
    }
}

