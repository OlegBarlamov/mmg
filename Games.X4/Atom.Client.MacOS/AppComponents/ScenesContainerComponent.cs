using System;
using Atom.Client.MacOS.Resources;
using Atom.Client.MacOS.Scenes;
using Atom.Client.MacOS.Services.Implementations;
using FrameworkSDK;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;

namespace Atom.Client.MacOS.AppComponents
{
    [UsedImplicitly]
    internal class ScenesContainerComponent : IAppComponent
    {
        private IScenesContainer ScenesContainer { get; }
        private ScenesResolverHolder ScenesResolverHolder { get; }

        public ScenesContainerComponent([NotNull] IScenesContainer scenesContainer, [NotNull] ScenesResolverHolder scenesResolverHolder)
        {
            ScenesContainer = scenesContainer ?? throw new ArgumentNullException(nameof(scenesContainer));
            ScenesResolverHolder = scenesResolverHolder ?? throw new ArgumentNullException(nameof(scenesResolverHolder));
        }
        
        public void Dispose()
        {
        }

        public void Configure()
        {
            ScenesContainer.RegisterScene<MainSceneDataModel, MainScene>();
            ScenesContainer.RegisterScene<LoadingSceneResources, LoadingScene>();

            ScenesResolverHolder.ScenesResolver = ScenesContainer.CreateResolver();
        }
    }
}