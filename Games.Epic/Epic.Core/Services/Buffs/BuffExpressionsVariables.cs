using System.Collections.Generic;
using Epic.Core.Utils;
using Epic.Data.Heroes;

namespace Epic.Core.Services.Buffs
{
    public class BuffExpressionsVariables : ExpressionVariables
    {
        public static BuffExpressionsVariables Empty() => new BuffExpressionsVariables(new Dictionary<string, double>());
        
        public static BuffExpressionsVariables FromVariables(ExpressionVariables variables) => 
            new BuffExpressionsVariables(variables.ToDictionary());

        public static BuffExpressionsVariables FromHero(IHeroStats stats) => Empty();

        private BuffExpressionsVariables(IReadOnlyDictionary<string, double> variables)
        {
            Variables = new Dictionary<string, double>(variables);
        }
    }
}