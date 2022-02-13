using FrameworkSDK;
using FrameworkSDK.Logging;
using JetBrains.Annotations;

namespace Logging.FrameworkAdapter
{
    public static class AppFactoryExtensions
    {
        public static DefaultAppFactory SetupLogSystem([NotNull] this DefaultAppFactory appFactory, string logDirectoryFullPath, bool isDebug = false, bool isFakeLog = false)
        {
            var config = new LogSystemConfig
            {
                LogDirectoryFullPath = logDirectoryFullPath,
                IsDebug = isDebug,
                IsFakeLog = isFakeLog
            };

            var logger = CreateLogger(config);
            return appFactory.UseLogger(logger);
        }

        private static IFrameworkLogger CreateLogger(LogSystemConfig config)
        {
            var logSystem = new LogSystem(config.LogDirectoryFullPath, config.IsDebug, config.IsFakeLog);
            return logSystem.ToFrameworkLogger();
        }

        private class LogSystemConfig
        {
            public string LogDirectoryFullPath { get; set; }
            public bool IsDebug { get; set; } = false;
            public bool IsFakeLog { get; set; } = false;
        }
    }
}