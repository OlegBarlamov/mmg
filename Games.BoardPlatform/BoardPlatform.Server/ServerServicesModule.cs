using BoardPlatform.Data.Repositories;
using BoardPlatform.Data.Tokens;
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
            serviceRegistrator.RegisterType<IBoardRepository, SimpleInMemoryBoardRepository>();
            serviceRegistrator.RegisterType<IWidgetRepository, SimpleInMemoryWidgetRepository>();
            serviceRegistrator.RegisterType<ITokensFactory, SimpleNumberTokensFactory>();
            serviceRegistrator.RegisterType<ITokensParser, SimpleTokensParser>();
            serviceRegistrator.RegisterType<IWebSocketConnectionsService, WebSocketConnectionsService>();
            serviceRegistrator.RegisterType<IBoardSocketsWorkersManager, BoardSocketsWorkersManager>();
        }
    }
}