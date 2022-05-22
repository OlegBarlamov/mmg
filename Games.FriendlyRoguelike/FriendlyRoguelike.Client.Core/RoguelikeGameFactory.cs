using System;
using System.IO;
using Autofac.FrameworkAdapter;
using Console.Core;
using Console.LoggingAdapter;
using FrameworkSDK;
using FrameworkSDK.Constructing;
using JetBrains.Annotations;
using Logging;
using Logging.FrameworkAdapter;

namespace FriendlyRoguelike.Core
{
    public class RoguelikeGameFactory : AppFactoryWrapper, IAppFactory
    {
        public IConsoleMessagesProvider ConsoleMessagesProvider { get; }

        private RoguelikeGameFactory([NotNull] IAppFactory appFactory, [NotNull] IConsoleMessagesProvider consoleMessagesProvider) : base(appFactory)
        {
            ConsoleMessagesProvider = consoleMessagesProvider ?? throw new ArgumentNullException(nameof(consoleMessagesProvider));
        }
        
        public static RoguelikeGameFactory Create([NotNull] RoguelikeGameFactoryConfig gameFactoryConfig)
        {
            if (gameFactoryConfig == null) throw new ArgumentNullException(nameof(gameFactoryConfig));

            var logConfig = gameFactoryConfig.LogginCoreConfig;
            var logSystemConfig = new LogSystemConfig
            {
                IsDebug = logConfig.IsDebug,
                LogDirectory = new DirectoryInfo(logConfig.LogDirectoryFullPath)
            };
            var logSystem = new LogSystem(logSystemConfig);
            var frameworkLogger = logSystem.ToFrameworkLogger();
            var consoleProvider = new LoggerConsoleMessagesProvider();
            logSystem.AddProvider(consoleProvider);

            var factory = new DefaultAppFactory()
                .UseLogger(frameworkLogger)
                .UseAutofac()
                .AddServices<RoguelikeGameMainModule>();
            
            return new RoguelikeGameFactory(factory, consoleProvider);
        }
    }
}