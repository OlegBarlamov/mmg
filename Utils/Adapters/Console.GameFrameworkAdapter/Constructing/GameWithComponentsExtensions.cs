using FrameworkSDK.MonoGame.Constructing;

namespace Console.FrameworkAdapter
{
    public static class GameWithExternalComponentsExtensions 
    {
        public static IGameWithConsoleFactory UseInGameConsole(this IGameFactoryWithExternalComponents gameFactory)
        {
            gameFactory.RegisterExternalGameComponent<ConsoleGameComponent>();
            var gameWithConsoleFactory = new GameWithConsoleFactory(gameFactory);
            gameWithConsoleFactory.AddServices<InGameConsoleServicesModule>();
            return gameWithConsoleFactory;
        }
    }
}