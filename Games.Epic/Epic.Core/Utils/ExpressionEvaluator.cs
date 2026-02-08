using System;
using System.Collections.Generic;
using DynamicExpresso;

namespace Epic.Core.Utils
{
    public static class ExpressionEvaluator
    {
        public static Interpreter CreateInterpreter(IReadOnlyDictionary<string, double> variables = null)
        {
            var vars = variables ?? new Dictionary<string, double>();
            var interpreter = new Interpreter();
            foreach (var kv in vars)
                interpreter.SetVariable(kv.Key, kv.Value);
            return interpreter;
        }
        
        public static int EvalInt(string expression, Interpreter interpreter)
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
