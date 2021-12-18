using FrameworkSDK.Constructing;

namespace FrameworkSDK.MonoGame.Constructing
{
    public static class GameFactory
    {
        public static IGameConfigurator<TGame> CreateGame<TGame>(this AppFactory appFactory) where TGame : GameApp
        {
            return appFactory.Create<TGame>()
                .UseGameFramework<TGame>();
        }

        public static IGameConfigurator<TGame> UseGame<TGame>(this IAppConfigurator appConfigurator)
            where TGame : GameApp
        {
            return appConfigurator.UseApplication<TGame>()
                .UseGameFramework<TGame>();
        }
    }
}
