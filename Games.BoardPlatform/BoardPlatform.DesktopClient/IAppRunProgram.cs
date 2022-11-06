using FrameworkSDK.DependencyInjection;

namespace TablePlatform.DesktopClient
{
    public interface IAppRunProgram
    {
        void RegisterCustomServices(IServiceRegistrator serviceRegistrator);
    }
}