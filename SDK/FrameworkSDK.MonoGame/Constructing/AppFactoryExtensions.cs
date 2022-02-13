using FrameworkSDK.MonoGame.Services;

namespace FrameworkSDK.MonoGame.Constructing
{
    public static class AppFactoryExtensions
    {
        public static IGameFactory UseGame<TGame>(this IAppFactory appFactory) where TGame : GameApp
        {
            var gameFactory = new GameFactory(appFactory);
            gameFactory.AddServices<GameModule<TGame>>();
            gameFactory.AddComponent<GameAppComponent>();
            return gameFactory;
        }

        public static IGameFactoryWithExternalComponents UseGameComponents(this IGameFactory gameFactory)
        {
            var gameFactoryWithExternalComponents = new GameFactoryWithExternalComponents(gameFactory);
            gameFactoryWithExternalComponents.AddServices<GameComponentsServicesModule>();
            gameFactoryWithExternalComponents.AddComponent<GameComponentsAppComponent>();
            return gameFactoryWithExternalComponents;
        }
    }
}