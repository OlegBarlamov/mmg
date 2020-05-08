using System;
using Autofac.FrameworkAdapter;
using Console.FrameworkAdapter;
using Console.LoggingAdapter;
using FrameworkSDK;
using FrameworkSDK.Constructing;
using FrameworkSDK.MonoGame.Config;
using FrameworkSDK.MonoGame.Constructing;
using FrameworkSDK.MonoGame.ExternalComponents;
using JetBrains.Annotations;
using Logging;
using Logging.FrameworkAdapter;
using TablePlatform.Client.Modules;

// ReSharper disable HeapView.CanAvoidClosure
namespace TablePlatform.Client
{
    public static class TablePlatformFactory
    {
        public static IAppRunner Create([NotNull] IAppRunProgram program)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            
            var logSystem = new LogSystem("Logs", true, true);
            var frameworkLogger = logSystem.ToFrameworkLogger();
            var consoleProvider = new LoggerConsoleMessagesProvider();
            logSystem.AddProvider(consoleProvider);
            
            return new AppFactory(frameworkLogger)
                .CreateGame<TablePlatformGame>()
                .SetupGameParameters(GetParameters)
                .UseComponents()
                    .AddConsole(consoleProvider, new CommandExecutorMediator())
                .SetupCustomLogger(() => frameworkLogger)
                .UseAutofac()
                .RegisterServices<MainModule>()
                .RegisterServices(program.RegisterCustomServices)
                .Configure();
        }

        private static IGameParameters GetParameters()
        {
            return new DefaultGameParameters
            {
                ContentRootDirectory = "Resources"
            };
        }
    }
}