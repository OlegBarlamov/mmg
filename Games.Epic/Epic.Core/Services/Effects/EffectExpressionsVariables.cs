using System.Collections.Generic;
using Epic.Core.Utils;

namespace Epic.Core.Services.Effects
{
    public class EffectExpressionsVariables : ExpressionVariables
    {
        public static EffectExpressionsVariables Empty() => new EffectExpressionsVariables(new  Dictionary<string, double>());
        
        public static EffectExpressionsVariables FromVariables(ExpressionVariables variables) =>
            new EffectExpressionsVariables(variables.ToDictionary());
        
        private EffectExpressionsVariables(IReadOnlyDictionary<string, double> variables)
        {
            Variables = new Dictionary<string, double>(variables);
        }
    }
}
