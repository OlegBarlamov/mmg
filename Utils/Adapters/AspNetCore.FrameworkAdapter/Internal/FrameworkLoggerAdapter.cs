using System;
using FrameworkSDK.Logging;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace AspNetCore.FrameworkAdapter
{
    internal class FrameworkLoggerAdapter : IFrameworkLogger
    {
        public ILoggerFactory LoggerFactory { get; }

        public FrameworkLoggerAdapter([NotNull] ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }
        
        public void Log(string message, string logCategory, FrameworkLogLevel level)
        {
            LoggerFactory.CreateLogger(logCategory).Log(ToLogLevel(level), message);
        }

        private LogLevel ToLogLevel(FrameworkLogLevel frameworkLogLevel)
        {
            switch (frameworkLogLevel)
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
                    throw new ArgumentOutOfRangeException(nameof(frameworkLogLevel), frameworkLogLevel, null);
            }
        }
    }
}