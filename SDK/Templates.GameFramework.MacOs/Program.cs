using System;
using Console.FrameworkAdapter;
using Console.FrameworkAdapter.Constructing;
using Console.GameFrameworkAdapter.Constructing;
using Console.LoggingAdapter;
using FrameworkSDK;
using FrameworkSDK.Constructing;
using FrameworkSDK.MonoGame.Config;
using FrameworkSDK.MonoGame.Constructing;
using FrameworkSDK.MonoGame.Resources.Generation;
using Logging.FrameworkAdapter;
using NetExtensions.Geometry;

namespace Template.MacOs
{
    internal class Program
    {
#if DEBUG
        private const bool IsDebug = true;
#else
        private const bool IsDebug = false;
#endif
        
        [STAThread]
        public static void Main()
        {
            var gameParameters = new DefaultGameParameters
            {
                BackBufferSize = new SizeInt(1280, 768),
                IsMouseVisible = false,
            };
            using (var gameFactory = GetFactory(IsDebug, gameParameters).Construct())
            {
                gameFactory.Run();
            }
        }
        
        private static IGameFactory GetFactory(bool isDebug, IGameParameters gameParameters)
        {
            var loggerConsoleMessageProvider = new LoggerConsoleMessagesProvider();
            return new DefaultAppFactory()
                .SetupLogSystem(loggerConsoleMessageProvider, isDebug)
                .AddServices<ConsoleCommandsExecutingServicesModule>()
                .UseGame<TemplateGameApp>()
                .UseGameParameters(gameParameters)
                .UseMvc()
                .PreloadResourcePackage<ColorsTexturesPackage>()
                .UseGameComponents()
                .UseInGameConsole()
                .UseConsoleMessagesProvider(loggerConsoleMessageProvider)
                .UseConsoleCommandExecutor<ExecutableConsoleCommandsExecutor>();
        }
    }
}