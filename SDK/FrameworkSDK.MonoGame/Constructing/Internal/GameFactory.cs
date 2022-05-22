using FrameworkSDK.Constructing;
using FrameworkSDK.MonoGame.Config;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.Resources;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Constructing
{
    internal class GameFactory : AppFactoryWrapper, IGameFactory
    {
        private bool _resourcePreloaderComponentRegistered;
        private readonly PreloadingResourcesCollection _preloadingResourcesCollection = new PreloadingResourcesCollection();

        public GameFactory([NotNull] IAppFactory appFactory)
            : base(appFactory)
        {
        }

        public IGameFactory UseGameParameters(IGameParameters gameParameters)
        {
            AppFactory.AddService(gameParameters);
            return this;
        }

        public IGameFactory PreloadResourcePackage(IResourcePackage resourcePackage)
        {
            if (!_resourcePreloaderComponentRegistered)
            {
                AppFactory.AddComponent<ResourcesPreloaderComponent>();
                AppFactory.AddService(_preloadingResourcesCollection);
                _resourcePreloaderComponentRegistered = true;
            }
            
            _preloadingResourcesCollection.Add(resourcePackage);
            AppFactory.AddServices(registrator =>
            {
                registrator.RegisterInstance(resourcePackage.GetType(), resourcePackage);
            });

            return this;
        }
    }
}