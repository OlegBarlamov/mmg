using System;
using FrameworkSDK.Logging;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Logging.FrameworkAdapter
{
    internal class FrameworkLoggerWrapper : IFrameworkLogger
    {
        private ILoggerFactory LoggerFactory { get; }

        public FrameworkLoggerWrapper([NotNull] ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }
        
        public void Log(string message, FrameworkLogModule module, FrameworkLogLevel level)
        {
            LoggerFactory.CreateLogger(module.ToString()).Log(message, ToLogLevel(level));
        }

        private static LogLevel ToLogLevel(FrameworkLogLevel logLevel)
        {
            switch (logLevel)
            {
                case FrameworkLogLevel.Trace:
                    return LogLevel.Trace;
                case FrameworkLogLevel.Debug:
                    return LogLevel.Debug;
                case FrameworkLogLevel.Info:
                    return LogLevel.Information;
                case FrameworkLogLevel.Warn:
                    return LogLevel.Warning;
                case FrameworkLogLevel.Error:
                    return LogLevel.Error;
                case FrameworkLogLevel.Fatal:
                    return LogLevel.Critical;
                default:
                    return LogLevel.Information;
            }
        }
    }
}