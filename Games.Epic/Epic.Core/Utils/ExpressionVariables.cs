using System.Collections.Generic;

namespace Epic.Core.Utils
{
    public abstract class ExpressionVariables
    {
        protected Dictionary<string, double>  Variables { get; set; } = new Dictionary<string, double>();
        public IReadOnlyDictionary<string, double> ToDictionary() => Variables; 
    }
}