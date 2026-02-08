using System.Collections.Generic;
using Epic.Data.Effect;
using Epic.Data.EffectType;
using Epic.Core.Utils;

namespace Epic.Core.Services.Effects
{
    public static class EffectExpressionEvaluator
    {
        public static EffectEffectiveValues Evaluate(
            IEffectTypeFields effectType,
            IReadOnlyDictionary<string, double> variables)
        {
            var interpreter = ExpressionEvaluator.CreateInterpreter(variables);
            return new EffectEffectiveValues
            {
                TakesDamageMin = ExpressionEvaluator.EvalInt(effectType.TakesDamageMinExpression, interpreter),
                TakesDamageMax = ExpressionEvaluator.EvalInt(effectType.TakesDamageMaxExpression, interpreter),
                Heals = ExpressionEvaluator.EvalInt(effectType.HealsExpression, interpreter),
                HealsPercentage = ExpressionEvaluator.EvalInt(effectType.HealsPercentageExpression, interpreter),
                HealCanResurrect = effectType.HealCanResurrect,
            };
        }
    }
}
