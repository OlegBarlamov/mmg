using System;
using FrameworkSDK.Constructing;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.Resources;
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

        public static IGameFactory PreloadResourcePackage<TResourcePackage>(this IGameFactory gameFactory) where TResourcePackage : class, IResourcePackage
        {
            var resourcePackageInstance = Activator.CreateInstance<TResourcePackage>();
            return gameFactory.PreloadResourcePackage(resourcePackageInstance);
        }

        public static IGameFactory UseMvc(this IGameFactory gameFactory)
        {
            gameFactory.AddServices<MvcServicesModule>();
            gameFactory.AddComponent<MvcAppComponent>();
            return gameFactory;
        }

        public static IGameFactory UseHighPolygonal(this IGameFactory gameFactory)
        {
            gameFactory.AddServices<HighPolygonalServicesModule>();
            return gameFactory;
        }
    }
}