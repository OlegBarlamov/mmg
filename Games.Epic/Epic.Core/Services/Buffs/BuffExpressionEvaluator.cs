using System;
using Epic.Data.Buff;
using Epic.Data.BuffType;
using DynamicExpresso;

namespace Epic.Core.Services.Buffs
{
    public static class BuffExpressionEvaluator
    {
        public static BuffEffectiveValues Evaluate(
            IBuffFields buffType,
            BuffExpressionsVariables variables)
        {
            var vars = variables.ToDictionary();
            var interpreter = new Interpreter();
            foreach (var kv in vars)
                interpreter.SetVariable(kv.Key, kv.Value);

            return new BuffEffectiveValues
            {
                HealthBonus = EvalInt(buffType.HealthBonusExpression, interpreter),
                AttackBonus = EvalInt(buffType.AttackBonusExpression, interpreter),
                DefenseBonus = EvalInt(buffType.DefenseBonusExpression, interpreter),
                SpeedBonus = EvalInt(buffType.SpeedBonusExpression, interpreter),
                MinDamageBonus = EvalInt(buffType.MinDamageBonusExpression, interpreter),
                MaxDamageBonus = EvalInt(buffType.MaxDamageBonusExpression, interpreter),
                HealthBonusPercentage = EvalInt(buffType.HealthBonusPercentageExpression, interpreter),
                AttackBonusPercentage = EvalInt(buffType.AttackBonusPercentageExpression, interpreter),
                DefenseBonusPercentage = EvalInt(buffType.DefenseBonusPercentageExpression, interpreter),
                SpeedBonusPercentage = EvalInt(buffType.SpeedBonusPercentageExpression, interpreter),
                MinDamageBonusPercentage = EvalInt(buffType.MinDamageBonusPercentageExpression, interpreter),
                MaxDamageBonusPercentage = EvalInt(buffType.MaxDamageBonusPercentageExpression, interpreter),
                Paralyzed = buffType.Paralyzed,
                Stunned = buffType.Stunned,
                VampirePercentage = EvalInt(buffType.VampirePercentageExpression, interpreter),
                VampireCanResurrect = buffType.VampireCanResurrect,
                DeclinesWhenTakesDamage = buffType.DeclinesWhenTakesDamage,
                Heals = EvalInt(buffType.HealsExpression, interpreter),
                HealsPercentage = EvalInt(buffType.HealsPercentageExpression, interpreter),
                HealCanResurrect = buffType.HealCanResurrect,
                TakesDamageMin = EvalInt(buffType.TakesDamageMinExpression, interpreter),
                TakesDamageMax = EvalInt(buffType.TakesDamageMaxExpression, interpreter),
                DamageReturnPercentage = EvalInt(buffType.DamageReturnPercentageExpression, interpreter),
                DamageReturnMaxRange = EvalInt(buffType.DamageReturnMaxRangeExpression, interpreter),
                Permanent = buffType.Permanent,
                Duration = EvalInt(buffType.DurationExpression, interpreter),
            };
        }

        private static int EvalInt(string expression, Interpreter interpreter)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return 0;
            try
            {
                var result = interpreter.Eval(expression);
                if (result == null)
                    return 0;
                if (result is int i)
                    return i;
                if (result is double d)
                    return (int)Math.Round(d);
                
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to evaluate expression \"{expression}\": {ex.Message}", ex);
            }
        }

    }
}
