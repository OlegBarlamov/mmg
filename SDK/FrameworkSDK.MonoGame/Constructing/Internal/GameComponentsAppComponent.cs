using System;
using FrameworkSDK.DependencyInjection;
using FrameworkSDK.MonoGame.ExternalComponents;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Constructing
{
    [UsedImplicitly]
    internal class GameComponentsAppComponent : IAppComponent
    {
        private IExternalGameComponentsService ExternalGameComponentsService { get; }
        private IServiceLocator ServiceLocator { get; }

        public GameComponentsAppComponent([NotNull] IExternalGameComponentsService externalGameComponentsService,
            [NotNull] IServiceLocator serviceLocator)
        {
            ExternalGameComponentsService = externalGameComponentsService ?? throw new ArgumentNullException(nameof(externalGameComponentsService));
            ServiceLocator = serviceLocator ?? throw new ArgumentNullException(nameof(serviceLocator));
        }
        
        public void Configure()
        {
            ExternalGameComponentsService.LoadComponents(ServiceLocator);
        }

        public void Dispose()
        {
        }
    }
}