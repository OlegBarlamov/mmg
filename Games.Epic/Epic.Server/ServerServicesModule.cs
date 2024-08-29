using Console.Core;
using Console.FrameworkAdapter;
using FrameworkSDK.DependencyInjection;

namespace Epic.Server
{
    public class ServerServicesModule : IServicesModule
    {
        public void RegisterServices(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.RegisterType<IConsoleController, LoggerConsoleMessagesViewer>();
        }
    }
}