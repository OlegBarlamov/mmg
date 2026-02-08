using System.Collections.Generic;

namespace Epic.Core.Utils
{
    public abstract class ExpressionVariables
    {
        public abstract IReadOnlyDictionary<string, double> ToDictionary(); 
    }
}