using FrameworkSDK.DependencyInjection;

namespace Atom.Client.MacOS
{
    public class MainModule : IServicesModule
    {
        public void RegisterServices(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.RegisterType<MainSceneDataModel, MainSceneDataModel>();
        }
    }
}