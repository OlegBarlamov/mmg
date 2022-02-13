using System;

namespace FrameworkSDK.Logging
{
    public class DefaultConsoleLogger : IFrameworkLogger
    {
        public void Log(string message, FrameworkLogModule module, FrameworkLogLevel level)
        {
            if (string.IsNullOrEmpty(message))
                return;

            var prefix = GetPrefix(module, level);
            var line = $"{prefix}{message}";
            Console.WriteLine(line);
        }

        private static string GetPrefix(FrameworkLogModule module, FrameworkLogLevel level)
        {
            var levelPrefix = GetLevelPrefix(level);
            return $"{levelPrefix}{module.ToString()}:";
        }

        private static string GetLevelPrefix(FrameworkLogLevel level)
        {
            if (level == FrameworkLogLevel.Fatal)
                return "FATAL!";
            if (level == FrameworkLogLevel.Error)
                return "Error!";
            if (level == FrameworkLogLevel.Warn)
                return "Warning.";
            return "";
        }
    }
}