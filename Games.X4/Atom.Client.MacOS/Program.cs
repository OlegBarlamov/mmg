using System;
using Atom.Client.MacOS.AppComponents;
using Atom.Client.MacOS.Resources;
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
                .SetupLogSystem(loggerConsoleMessageProvider, true)
                .AddComponent<ScenesContainerComponent>()
                .AddServices<MainServicesModule>()
                .UseGame<X4GameApp>()
                    .UseMvc()
                    .PreloadResourcePackage<ColorsTexturesPackage>()
                    .PreloadResourcePackage<LoadingSceneResources>()
                    .UseGameComponents()
                        .UseInGameConsole()
                        .UseConsoleMessagesProvider(loggerConsoleMessageProvider)
                        .UseConsoleCommandExecutor<ExecutableConsoleCommandsExecutor>()
                .Construct())
            {
                game.Run();
            }
        }
    }
}