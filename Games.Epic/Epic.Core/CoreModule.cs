using Epic.Core.Services;
using Epic.Core.Services.Implementations;
using FrameworkSDK.IoC;

namespace Epic.Core
{
    public class CoreModule : IServicesModule
    {
        public void Register(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.RegisterType<IConsoleService, ConsoleService>();
        }
    }
}
