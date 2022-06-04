using Atom.Client.MacOS.Resources;
using Atom.Client.MacOS.Services;
using Atom.Client.MacOS.Services.Implementations;
using FrameworkSDK.DependencyInjection;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;

namespace Atom.Client.MacOS
{
    [UsedImplicitly]
    public class MainServicesModule : IServicesModule
    {
        public void RegisterServices(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.RegisterType<MainSceneDataModel, MainSceneDataModel>();
            serviceRegistrator.RegisterType<IAstronomicMapGenerator, DefaultAstronomicMapGenerator>();
            serviceRegistrator.RegisterInstance(new ScenesResolverHolder());
            serviceRegistrator.RegisterFactory<IScenesResolver>(((locator, type) => locator.Resolve<ScenesResolverHolder>().ScenesResolver));
            
            serviceRegistrator.RegisterType<MainResourcePackage, MainResourcePackage>();
        }
    }
}