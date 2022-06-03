using System;
using Console.FrameworkAdapter;
using Console.LoggingAdapter;
using FrameworkSDK;
using FrameworkSDK.Constructing;
using FrameworkSDK.MonoGame.Constructing;
using FrameworkSDK.MonoGame.Resources.Generation;
using Logging.FrameworkAdapter;
using Microsoft.Extensions.Logging;

namespace Atom.Client.MacOS
{
    internal class Program
    {
        [STAThread]
        public static void Main()
        {
            var loggerConsoleMessageProvider = new LoggerConsoleMessagesProvider();
            var commandExecutor = new CommandExecutorMediator();
            using (var game = new DefaultAppFactory()
                .SetupLogSystem(new ILoggerProvider[]{loggerConsoleMessageProvider})
                .AddServices<MainModule>()
                .AddService(commandExecutor)
                .UseGame<X4GameApp>()
                    .UseMvc()
                    .PreloadResourcePackage<ColorsTexturesPackage>()
                    .PreloadResourcePackage<X4GameResourcePackage>()
                    .UseGameComponents()
                        .UseInGameConsole()
                        .UseConsoleMessagesProvider(loggerConsoleMessageProvider)
                        .UseConsoleCommandExecutor(commandExecutor)
                .Construct())
            {
                game.Run();
            }
        }
    }
}