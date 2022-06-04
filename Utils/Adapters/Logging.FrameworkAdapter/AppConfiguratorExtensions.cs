using System.IO;
using FrameworkSDK;
using FrameworkSDK.DependencyInjection;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Logging.FrameworkAdapter
{
    public static class AppFactoryExtensions
    {
        public const string DefaultLogDirectoryName = "Logs";

        public static DefaultAppFactory SetupLogSystem([NotNull] this DefaultAppFactory appFactory,
            bool isDebug = false)
        {
            return SetupLogSystem(appFactory, new ILoggerProvider[0], isDebug);
        }

        public static DefaultAppFactory SetupLogSystem([NotNull] this DefaultAppFactory appFactory,
            ILoggerProvider loggerProvider, bool isDebug = false)
        {
            return appFactory.SetupLogSystem(new[] {loggerProvider}, isDebug);
        }

        public static DefaultAppFactory SetupLogSystem([NotNull] this DefaultAppFactory appFactory, ILoggerProvider[] loggerProviders, bool isDebug = false)
        {
            var config = new LogSystemConfig
            {
                IsDebug = isDebug,
                LogDirectory = new DirectoryInfo(GetRelativeLogPath(DefaultLogDirectoryName)),
            };

            var logSystem = new LogSystem(config);
            foreach (var loggerProvider in loggerProviders)
            {
                logSystem.AddProvider(loggerProvider);
            }
            appFactory.AddServices(new ServicesModuleDelegate(registrator =>
            {
                registrator.RegisterInstance<ILoggerFactory>(logSystem);
            }));
            
            var logger = logSystem.ToFrameworkLogger();
            return appFactory.UseLogger(logger);
        }

        private static string GetRelativeLogPath(string relativeLogPath)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), relativeLogPath);
        }
    }
}