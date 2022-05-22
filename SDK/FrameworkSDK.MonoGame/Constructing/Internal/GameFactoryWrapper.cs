using System;
using FrameworkSDK.DependencyInjection;
using FrameworkSDK.MonoGame.Config;
using FrameworkSDK.MonoGame.Resources;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Constructing
{
    public class GameFactoryWrapper : IGameFactory
    {
        protected IGameFactory GameFactory { get; }

        public GameFactoryWrapper([NotNull] IGameFactory gameFactory)
        {
            GameFactory = gameFactory ?? throw new ArgumentNullException(nameof(gameFactory));
        }
        public IApp Construct()
        {
            return GameFactory.Construct();
        }

        public IAppFactory AddServices(IServicesModule module)
        {
            return GameFactory.AddServices(module);
        }

        public IAppFactory AddComponent<TComponent>() where TComponent : class, IAppComponent
        {
            return GameFactory.AddComponent<TComponent>();
        }

        public IAppFactory AddComponent(IAppComponent appComponent)
        {
            return GameFactory.AddComponent(appComponent);
        }

        public IGameFactory UseGameParameters(IGameParameters gameParameters)
        {
            return GameFactory.UseGameParameters(gameParameters);
        }

        public IGameFactory PreloadResourcePackage(IResourcePackage resourcePackage)
        {
            return GameFactory.PreloadResourcePackage(resourcePackage);
        }
    }
}