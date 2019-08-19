using System;
using FrameworkSDK;
using FrameworkSDK.Constructing;
using FrameworkSDK.Logging;
using Logging;
using Microsoft.Extensions.Logging;

namespace HeroesGame
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var app = App.Construct()
                .UseGameFramework<TestApplication>()
                .SetupCustomLogger(() => new MyLogger()))
            {
                app.Run();
            }
		}
    }

    internal class MyLogger : IFrameworkLogger
    {
        private readonly LogSystem _logSystem = new LogSystem("Log", true);

        public void Log(string message, FrameworkLogModule module, FrameworkLogLevel level)
        {
            var logger = _logSystem.CreateLogger(module.ToString());
            logger.Log(message, ToLogLevel(level));
        }

        private static LogLevel ToLogLevel(FrameworkLogLevel frameworkLogLevel)
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
#endif
}
