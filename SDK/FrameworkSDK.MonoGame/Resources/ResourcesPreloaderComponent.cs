using System;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Resources
{
    [UsedImplicitly]
    internal class ResourcesPreloaderComponent : IAppComponent
    {
        private IResourcesService ResourcesService { get; }
        private PreloadingResourcesCollection PreloadingResourcesCollection { get; }

        public ResourcesPreloaderComponent([NotNull] IResourcesService resourcesService,
            [NotNull] PreloadingResourcesCollection preloadingResourcesCollection)
        {
            ResourcesService = resourcesService ?? throw new ArgumentNullException(nameof(resourcesService));
            PreloadingResourcesCollection = preloadingResourcesCollection ?? throw new ArgumentNullException(nameof(preloadingResourcesCollection));
        }
        
        public void Configure()
        {
            foreach (var resource in PreloadingResourcesCollection.Resources)
            {
                ResourcesService.LoadPackage(resource, false);
            }
        }
        
        public void Dispose()
        {
            foreach (var resource in PreloadingResourcesCollection.Resources)
            {
                if (resource.IsLoaded)
                {
                    ResourcesService.UnloadPackage(resource, false);
                }
            }
            PreloadingResourcesCollection.Clear();
        }
    }
}