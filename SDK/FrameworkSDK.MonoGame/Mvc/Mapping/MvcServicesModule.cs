using FrameworkSDK.DependencyInjection;

namespace FrameworkSDK.MonoGame.Mvc
{
    public class MvcServicesModule : IServicesModule
    {
        public void RegisterServices(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.RegisterType<IViewsProvider, DefaultViewsProvider>();
            serviceRegistrator.RegisterType<IControllersProvider, DefaultControllersProvider>();
            serviceRegistrator.RegisterType<IMvcMappingProvider, DefaultMvcMappingProvider>();
            
            serviceRegistrator.RegisterType<IMvcMappingResolver, DefaultMvcMappingResolver>();
            serviceRegistrator.RegisterType<IMvcStrategyService, DefaultMvcStrategy>();
            
            serviceRegistrator.RegisterType<IScenesContainer, DefaultScenesContainer>();
        }
    }
}