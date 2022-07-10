using Atom.Client.AppComponents;
using Atom.Client.Resources;
using Console.FrameworkAdapter;
using Console.LoggingAdapter;
using FrameworkSDK;
using FrameworkSDK.Constructing;
using FrameworkSDK.MonoGame.Config;
using FrameworkSDK.MonoGame.Constructing;
using FrameworkSDK.MonoGame.Resources.Generation;
using Logging.FrameworkAdapter;

namespace Atom.Client
{
    public static class X4GameFactory
    {
        public static IGameFactory GetFactory(bool isDebug, IGameParameters gameParameters)
        {
            var loggerConsoleMessageProvider = new LoggerConsoleMessagesProvider();
            return new DefaultAppFactory()
                .SetupLogSystem(loggerConsoleMessageProvider, isDebug)
                .AddComponent<ScenesContainerComponent>()
                .AddServices<MainServicesModule>()
                .UseGame<X4GameApp>()
                .UseGameParameters(gameParameters)
                .UseMvc()
                .PreloadResourcePackage<ColorsTexturesPackage>()
                .PreloadResourcePackage<LoadingSceneResources>()
                .UseGameComponents()
                .UseInGameConsole()
                .UseConsoleMessagesProvider(loggerConsoleMessageProvider)
                .UseConsoleCommandExecutor<ExecutableConsoleCommandsExecutor>();
        }
    }
}