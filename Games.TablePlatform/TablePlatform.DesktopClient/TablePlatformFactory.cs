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
using TablePlatform.DesktopClient.Modules;

// ReSharper disable HeapView.CanAvoidClosure
namespace TablePlatform.DesktopClient
{
    public static class TablePlatformFactory
    {
        public static IAppFactory Create([NotNull] IAppRunProgram program)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            
            var logSystem = new LogSystem(new LogSystemConfig());
            var frameworkLogger = logSystem.ToFrameworkLogger();
            var consoleProvider = new LoggerConsoleMessagesProvider();
            logSystem.AddProvider(consoleProvider);

            return new DefaultAppFactory()
                .UseLogger(frameworkLogger)
                .UseAutofac()
                .AddServices<MainModule>()
                .AddServices(program.RegisterCustomServices)
                .UseGame<TablePlatformGame>()
                .UseGameParameters(GetParameters())
                .UseGameComponents()
                .UseInGameConsole()
                .UseConsoleMessagesProvider(consoleProvider)
                .UseConsoleCommandExecutor(new CommandExecutorMediator());
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