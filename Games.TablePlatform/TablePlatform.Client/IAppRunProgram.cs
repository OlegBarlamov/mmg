using FrameworkSDK.IoC;

namespace TablePlatform.Client
{
    public interface IAppRunProgram
    {
        void RegisterCustomServices(IServiceRegistrator serviceRegistrator);
    }
}