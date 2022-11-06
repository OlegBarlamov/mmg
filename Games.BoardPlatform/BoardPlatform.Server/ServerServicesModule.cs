using BoardPlatform.Server.Services;
using FrameworkSDK.DependencyInjection;

namespace BoardPlatform.Server
{
    internal class ServerServicesModule : IServicesModule
    {
        public void RegisterServices(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.RegisterType<IWebSocketsService, WebSocketsService>();
        }
    }
}