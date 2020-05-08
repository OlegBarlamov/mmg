using System;
using FrameworkSDK.MonoGame.Config;
using FrameworkSDK.MonoGame.Core;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Resources
{
    [UsedImplicitly]
    internal class ContentContainersFactory : IContentContainersFactory
    {
        [NotNull] private IGameParameters GameParameters { get; }
        [NotNull] private IGameHeartServices HeartServices { get; }
        [NotNull] private IResourceReferencesService ResourceReferencesService { get; }

        public ContentContainersFactory([NotNull] IGameParameters gameParameters,
            [NotNull] IGameHeartServices heartServices,
            [NotNull] IResourceReferencesService resourceReferencesService)
        {
            GameParameters = gameParameters ?? throw new ArgumentNullException(nameof(gameParameters));
            HeartServices = heartServices ?? throw new ArgumentNullException(nameof(heartServices));
            ResourceReferencesService = resourceReferencesService ?? throw new ArgumentNullException(nameof(resourceReferencesService));
        }
        
        public IContentContainer Create([NotNull] IResourcePackage package)
        {
            if (package == null) throw new ArgumentNullException(nameof(package));
            return new ContentContainer(GameParameters, HeartServices, ResourceReferencesService, package);
        }
    }
}