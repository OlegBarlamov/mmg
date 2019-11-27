using FrameworkSDK.IoC;
using Gates.ClientCore.ExternalCommands;
using Gates.ClientCore.Rooms;
using Gates.ClientCore.ServerConnection;

namespace Gates.ClientCore
{
    internal class GameCoreModule : IServicesModule
    {
        public void Register(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.RegisterType<IExternalCommandParser, ExternalCommandParser>();
            serviceRegistrator.RegisterType<IExternalCommandsProcessor, ExternalCommandsProcessor>();
            serviceRegistrator.RegisterType<IRoomController, RoomController>();
            serviceRegistrator.RegisterType<IClientHost, ClientHost>();
            serviceRegistrator.RegisterType<IServerConnector, FakeServerConnector>();
        }
    }
}