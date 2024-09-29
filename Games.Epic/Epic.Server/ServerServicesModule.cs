using Console.Core;
using Console.FrameworkAdapter;
using Epic.Core;
using Epic.Data;
using Epic.Data.UserUnits;
using Epic.Server.Services;
using FrameworkSDK.DependencyInjection;

namespace Epic.Server
{
    public class ServerServicesModule : IServicesModule
    {
        public void RegisterServices(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.RegisterType<IConsoleController, LoggerConsoleMessagesViewer>();
            
            serviceRegistrator.RegisterType<ISessionsService, DefaultSessionService>();
            serviceRegistrator.RegisterType<IUsersService, DefaultUsersService>();
            serviceRegistrator.RegisterType<IAuthorizationService, DefaultAuthorizationService>();
            serviceRegistrator.RegisterType<IUserUnitsService, DefaultUserUnitsService>();
            
            serviceRegistrator.RegisterType<ISessionsRepository, InMemorySessionsRepository>();
            serviceRegistrator.RegisterType<IUsersRepository, InMemoryUsersRepository>();
            serviceRegistrator.RegisterType<IUserUnitsRepository, InMemoryUserUnitsRepository>();
        }
    }
}