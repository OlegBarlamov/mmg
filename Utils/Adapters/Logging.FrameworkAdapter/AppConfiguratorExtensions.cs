using System.IO;
using FrameworkSDK;
using FrameworkSDK.Logging;
using JetBrains.Annotations;

namespace Logging.FrameworkAdapter
{
    public static class AppFactoryExtensions
    {
        public const string DefaultLogDirectoryName = "Logs";
        public static DefaultAppFactory SetupLogSystem([NotNull] this DefaultAppFactory appFactory, bool isDebug = false)
        {
            var config = new LogSystemConfig
            {
                IsDebug = isDebug,
                LogDirectory = new DirectoryInfo(GetRelativeLogPath(DefaultLogDirectoryName)),
            };

            var logger = CreateLogger(config);
            return appFactory.UseLogger(logger);
        }

        private static IFrameworkLogger CreateLogger(LogSystemConfig config)
        {
            var logSystem = new LogSystem(config);
            return logSystem.ToFrameworkLogger();
        }

        private static string GetRelativeLogPath(string relativeLogPath)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), relativeLogPath);
        }
    }
}