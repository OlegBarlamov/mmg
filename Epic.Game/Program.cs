using System;
using ConsoleWindow;
using FrameworkSDK;
using FrameworkSDK.Constructing;
using FrameworkSDK.IoC;
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
        private static readonly MyLogger MyLogger = new MyLogger();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var app = App.Construct()
                .UseGameFramework<TestApplication>()
                .SetupCustomLogger(() => MyLogger)
                .RegisterServices(RegisterServices))
            {
                app.Run();
            }
		}

        private static void RegisterServices(IServiceRegistrator serviceRegistrator)
        {
            var console = new ConsoleService();
            MyLogger.LogSystem.AddProvider(console);
            serviceRegistrator.RegisterInstance(console);
        }
    }

    internal class ConsoleService : ILoggerProvider
    {
        private readonly IConsoleHost _consoleHost;

        public ConsoleService()
        {
            _consoleHost = ConsoleFactory.CreateHosted();
            _consoleHost.Show();
        }

        public void Dispose()
        {
            _consoleHost.Dispose();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _consoleHost.CreateLogger(categoryName);
        }
    }

    internal class MyLogger : IFrameworkLogger
    {
        public LogSystem LogSystem { get; } = new LogSystem("Log", true);

        public void Log(string message, FrameworkLogModule module, FrameworkLogLevel level)
        {
            var logger = LogSystem.CreateLogger(module.ToString());
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
