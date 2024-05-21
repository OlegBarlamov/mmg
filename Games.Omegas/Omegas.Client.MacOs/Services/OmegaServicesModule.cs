using FrameworkSDK.DependencyInjection;
using Omegas.Client.MacOs.Models;
using Omegas.Client.MacOs.Services;

namespace Omegas.Client.MacOs
{
    public class OmegaServicesModule : IServicesModule
    {
        public void RegisterServices(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.RegisterType<MainScene, MainScene>();
            serviceRegistrator.RegisterType<OmegaGameService, OmegaGameService>();
            serviceRegistrator.RegisterType<MapObjectsGenerator, MapObjectsGenerator>();
            serviceRegistrator.RegisterType<SpheresColorsService, SpheresColorsService>();
        }
    }
}