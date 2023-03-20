using FrameworkSDK.DependencyInjection;

namespace River.Client.MacOS
{
    internal class MainModule : IServicesModule
    {
        public void RegisterServices(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.RegisterType<MainScene, MainScene>();
            serviceRegistrator.RegisterInstance(RiverMap.Generate(40, 30));
        }
    }
}