using System.Collections.Generic;
using System.Collections.ObjectModel;
using Epic.Core.Utils;

namespace Epic.Core.Services.Effects
{
    public class EffectExpressionsVariables : ExpressionVariables
    {
        public static EffectExpressionsVariables Empty() => new EffectExpressionsVariables();

        public override IReadOnlyDictionary<string, double> ToDictionary()
        {
            return new ReadOnlyDictionary<string, double>(new Dictionary<string, double>());
        }
    }
}
