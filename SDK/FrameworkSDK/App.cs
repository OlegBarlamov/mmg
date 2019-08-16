using System;
using FrameworkSDK.Constructing;
using FrameworkSDK.Logging;
using FrameworkSDK.Pipelines;
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

            var defaultProcessor = new AppConstructor();
            var configureHandler = new AppConfigureHandler(defaultProcessor)
            {
                ConfigurationPipeline = configuration
            };

            return configureHandler;
        }

        /// <summary>
        /// Сконструировать приложение на основе существующей конфигурации
        /// </summary>
        public static IAppConfigureHandler ConstructCustom([NotNull] Pipeline appConfiguration, IFrameworkLogger constructlogger = null)
        {
            if (appConfiguration == null) throw new ArgumentNullException(nameof(appConfiguration));

            var defaultProcessor = new AppConstructor(constructlogger);
            var configureHandler = new AppConfigureHandler(defaultProcessor)
            {
                ConfigurationPipeline = appConfiguration
            };

            return configureHandler;
        }

        /// <summary>
        /// Сконструировать приложение на основе существующей конфигурации, используя свой построитель.
        /// </summary>
        [NotNull]
        public static IAppConfigureHandler ConstructCustom([NotNull] Pipeline appConfiguration,
            [NotNull] IPipelineProcessor pipelineProcessor)
        {
            if (appConfiguration == null) throw new ArgumentNullException(nameof(appConfiguration));
            if (pipelineProcessor == null) throw new ArgumentNullException(nameof(pipelineProcessor));

            var configureHandler = new AppConfigureHandler(pipelineProcessor)
            {
                ConfigurationPipeline = appConfiguration
            };

            return configureHandler;
        }
    }
}