using System;
using Console.Core;
using FrameworkSDK;
using FrameworkSDK.Constructing;
using FrameworkSDK.IoC;
using FrameworkSDK.Logging;
using FrameworkSDK.MonoGame.Constructing;
using JetBrains.Annotations;
using Logging;
using TablePlatform.Client.Console;
using TablePlatform.Client.IoC;
using TablePlatform.Client.Logging;
using TablePlatform.Client.Modules;

namespace TablePlatform.Client
{
    public static class TablePlatformFactory
    {
        public static IAppRunner Create([NotNull] IAppRunProgram program)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            var context = new ConstructionContext(program);
            return new AppFactory()
                .CreateGame<TablePlatformGame>()
                .SetupCustomLogger(context, InitializeLogSystem)
                .SetupCustomIoc(context, InitializeIocSystem)
                .RegisterServices(context, RegisterServices)
                .Configure();
        }
        
        private static IFrameworkLogger InitializeLogSystem(ConstructionContext context)
        {
            var logSystem = new LogSystem("Logs", true, true);
            var consoleSystem = ConsoleFactory.CreateConsole();
            context.ConsoleSystem = consoleSystem;
            logSystem.AddProvider(consoleSystem.ConsoleMessagesProvider);
            return new FrameworkLoggerWrapper(logSystem);
        }

        private static void RegisterServices(ConstructionContext context, IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.RegisterInstance<IConsoleMessagesProvider>(context.ConsoleSystem.ConsoleMessagesProvider);
            serviceRegistrator.RegisterInstance<IConsoleCommandExecutor>(context.ConsoleSystem.ConsoleCommandExecutor);
            //serviceRegistrator.RegisterInstance(context.ConsoleSystem.ConsoleController);

            var module = new MainModule();
            serviceRegistrator.RegisterModule(module);
            
            context.Program.RegisterCustomServices(serviceRegistrator);
        }

        private static IServiceContainerFactory InitializeIocSystem(ConstructionContext context)
        {
            return new AutofacServiceContainerFactory();
        }

        private class ConstructionContext
        {
            public IAppRunProgram Program { get; }
            
            public ConsoleWrapper ConsoleSystem { get; set; }

            public ConstructionContext([NotNull] IAppRunProgram program)
            {
                Program = program ?? throw new ArgumentNullException(nameof(program));
            }
        }
    }
}