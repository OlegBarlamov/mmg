using Serilog;
using Serilog.Core;

namespace Logging
{
    internal static class LoggerConfigurationExtension
    {
        public static LoggerConfiguration SetupBase(this LoggerConfiguration loggerConfiguration, bool isDebug)
        {
            if (isDebug)
                loggerConfiguration.MinimumLevel.Verbose();
            else
                loggerConfiguration.MinimumLevel.Information();

            return loggerConfiguration;
        }

        public static LoggerConfiguration WriteToFile(this LoggerConfiguration loggerConfiguration, string logFilePathFormatted)
        {
            return loggerConfiguration.WriteTo.RollingFile(logFilePathFormatted, outputTemplate: "{Message}{NewLine}");
        }

        public static LoggerConfiguration WriteToLoggers(this LoggerConfiguration loggerConfiguration, params Logger[] loggers)
        {
            var config = loggerConfiguration;
            foreach (var logger in loggers)
            {
                config = config.WriteTo.Logger(logger);
            }

            return config;
        }
    }
}
