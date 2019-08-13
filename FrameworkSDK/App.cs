using System;
using FrameworkSDK.Configuration;
using FrameworkSDK.Constructing;
using FrameworkSDK.Logging;
using JetBrains.Annotations;

namespace FrameworkSDK
{
    public static class App
    {
        /// <summary>
        /// Сконструировать приложение по умолчанию
        /// </summary>
        public static IAppConfigurator Construct()
        {
            var defaultConfigurationFactory = new DefaultConfigurationFactory();
            var configuration = defaultConfigurationFactory.Create();

            var defaultProcessor = new AppConfigurationProcessor();
            var configureHandler = new AppConfigureHandler(defaultProcessor)
            {
                Configuration = configuration
            };

            return configureHandler;
        }

        /// <summary>
        /// Сконструировать приложение на основе существующей конфигурации
        /// </summary>
        public static IAppConfigureHandler ConstructCustom([NotNull] PhaseConfiguration appConfiguration, IFrameworkLogger logger = null)
        {
            if (appConfiguration == null) throw new ArgumentNullException(nameof(appConfiguration));

            var defaultProcessor = new AppConfigurationProcessor();
            var configureHandler = new AppConfigureHandler(defaultProcessor)
            {
                Configuration = appConfiguration
            };

            return configureHandler;
        }

        /// <summary>
        /// Сконструировать приложение на основе существующей конфигурации, используя свой построитель.
        /// </summary>
        [NotNull]
        public static IAppConfigureHandler ConstructCustom([NotNull] PhaseConfiguration appConfiguration,
            [NotNull] IConfigurationProcessor configurationProcessor)
        {
            if (appConfiguration == null) throw new ArgumentNullException(nameof(appConfiguration));
            if (configurationProcessor == null) throw new ArgumentNullException(nameof(configurationProcessor));

            var configureHandler = new AppConfigureHandler(configurationProcessor)
            {
                Configuration = appConfiguration
            };

            return configureHandler;
        }
    }
}