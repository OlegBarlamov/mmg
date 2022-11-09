using BoardPlatform.Server.Services;
using Console.Core;
using Console.FrameworkAdapter;
using FrameworkSDK.DependencyInjection;

namespace BoardPlatform.Server
{
    internal class ServerServicesModule : IServicesModule
    {
        public void RegisterServices(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.RegisterType<IConsoleController, LoggerConsoleMessagesViewer>();
            
            serviceRegistrator.RegisterType<IWebSocketsService, WebSocketsService>();
        }
    }
}