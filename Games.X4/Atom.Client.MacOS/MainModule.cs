using System.Net.Mail;
using Atom.Client.MacOS.Components;
using FrameworkSDK.DependencyInjection;

namespace Atom.Client.MacOS
{
    public class MainModule : IServicesModule
    {
        public void RegisterServices(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.RegisterType<MainSceneDataModel, MainSceneDataModel>();
            serviceRegistrator.RegisterType<FirstPersonCameraProvider, FirstPersonCameraProvider>();
            serviceRegistrator.RegisterType<AstronomicalMapGenerator, AstronomicalMapGenerator>();
        }
    }
}