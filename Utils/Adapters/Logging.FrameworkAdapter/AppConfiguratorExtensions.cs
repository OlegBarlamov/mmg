using System;
using FrameworkSDK.Constructing;
using FrameworkSDK.Logging;
using JetBrains.Annotations;

namespace Logging.FrameworkAdapter
{
    public static class AppConfiguratorExtensions
    {
        public static IAppConfigurator SetupLogSystem([NotNull] this IAppConfigurator appConfigurator, string logDirectoryFullPath, bool isDebug = false, bool isFakeLog = false)
        {
            if (appConfigurator == null) throw new ArgumentNullException(nameof(appConfigurator));
            
            var config = new LogSystemConfig
            {
                LogDirectoryFullPath = logDirectoryFullPath,
                IsDebug = isDebug,
                IsFakeLog = isFakeLog
            };
            
            return appConfigurator.SetupCustomLogger(config, CreateLogger);
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