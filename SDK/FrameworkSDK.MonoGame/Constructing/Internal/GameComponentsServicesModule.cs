using FrameworkSDK.DependencyInjection;
using FrameworkSDK.MonoGame.ExternalComponents;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Constructing
{
    [UsedImplicitly]
    internal class GameComponentsServicesModule : IServicesModule
    {
        public void RegisterServices(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.RegisterType<IExternalGameComponentsService, ExternalGameComponentsService>();
        }
    }
}