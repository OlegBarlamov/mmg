using System.Collections.Generic;
using System.Collections.ObjectModel;
using Epic.Core.Utils;
using Epic.Data.Heroes;

namespace Epic.Core.Services.Buffs
{
    public class BuffExpressionsVariables : ExpressionVariables
    {
        public static BuffExpressionsVariables Empty() => new BuffExpressionsVariables();
        
        public static BuffExpressionsVariables FromHero(IHeroStats stats) => new BuffExpressionsVariables();
        
        public override IReadOnlyDictionary<string, double> ToDictionary()
        {
            return new ReadOnlyDictionary<string, double>(new Dictionary<string, double>());
        }
    }
}