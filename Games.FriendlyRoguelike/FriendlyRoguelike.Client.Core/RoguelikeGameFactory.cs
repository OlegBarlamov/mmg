using System;
using Autofac.FrameworkAdapter;
using Console.LoggingAdapter;
using FrameworkSDK;
using FrameworkSDK.Constructing;
using JetBrains.Annotations;
using Logging;
using Logging.FrameworkAdapter;

namespace FriendlyRoguelike.Core
{
    public static class RoguelikeGameFactory
    {
        public static RoguelikeGameConfigurator Create([NotNull] RoguelikeGameFactoryConfig gameFactoryConfig)
        {
            if (gameFactoryConfig == null) throw new ArgumentNullException(nameof(gameFactoryConfig));

            var logConfig = gameFactoryConfig.LogginCoreConfig;
            var logSystem = new LogSystem(logConfig.LogDirectoryFullPath, logConfig.IsDebug, logConfig.FakeLog);
            var frameworkLogger = logSystem.ToFrameworkLogger();
            var consoleProvider = new LoggerConsoleMessagesProvider();
            logSystem.AddProvider(consoleProvider);

            return new AppFactory(frameworkLogger)
                .Create()
                .SetupCustomLogger(frameworkLogger)
                .UseAutofac()
                .RegisterServices<RoguelikeGameMainModule>()
                .WrapConfigurator(config => new RoguelikeGameConfigurator(config, consoleProvider));
        } 
    }
}