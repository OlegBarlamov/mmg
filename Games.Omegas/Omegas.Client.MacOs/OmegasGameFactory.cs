using Console.FrameworkAdapter;
using Console.FrameworkAdapter.Constructing;
using Console.GameFrameworkAdapter.Constructing;
using Console.LoggingAdapter;
using FrameworkSDK;
using FrameworkSDK.Constructing;
using FrameworkSDK.MonoGame.Config;
using FrameworkSDK.MonoGame.Constructing;
using FrameworkSDK.MonoGame.Physics2D;
using FrameworkSDK.MonoGame.Resources.Generation;
using Logging.FrameworkAdapter;
using SimplePhysics2D.Module;

namespace Omegas.Client.MacOs
{
    internal static class OmegasGameFactory
    {
        public static IGameFactory GetFactory(bool isDebug, IGameParameters gameParameters)
        {
            var loggerConsoleMessageProvider = new LoggerConsoleMessagesProvider();
            return new DefaultAppFactory()
                .SetupLogSystem(loggerConsoleMessageProvider, isDebug)
                .AddServices<ConsoleCommandsExecutingServicesModule>()
                .AddService<MainScene>()
                .UseGame<OmegasGameApp>()
                .UseGameParameters(gameParameters)
                .UseMvc()
                .UsePhysics(new SimplePhysicsScene2DParameters())
                .PreloadResourcePackage<ColorsTexturesPackage>()
                .PreloadResourcePackage<GameResourcePackage>()
                .UseGameComponents()
                .UseInGameConsole()
                .UseConsoleMessagesProvider(loggerConsoleMessageProvider)
                .UseConsoleCommandExecutor<ExecutableConsoleCommandsExecutor>();
        }
    }
}