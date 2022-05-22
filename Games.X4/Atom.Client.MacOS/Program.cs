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
            using (var game = new DefaultAppFactory()
                .SetupLogSystem(new ILoggerProvider[]{loggerConsoleMessageProvider})
                .UseGame<X4GameApp>()
                .PreloadResourcePackage<ColorsTexturesPackage>()
                .UseGameComponents()
                .UseInGameConsole()
                .UseConsoleMessagesProvider(loggerConsoleMessageProvider)
                .AddServices<MainModule>()
                .Construct())
            {
                game.Run();
            }
        }
    }
}