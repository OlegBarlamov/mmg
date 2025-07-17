using System.Net.WebSockets;
using Console.Core;
using Console.FrameworkAdapter;
using Epic.Core;
using Epic.Core.Services;
using Epic.Data;
using Epic.Data.BattleDefinitions;
using Epic.Data.Battles;
using Epic.Data.BattleUnits;
using Epic.Data.Reward;
using Epic.Data.UnitTypes;
using Epic.Data.UserUnits;
using Epic.Logic;
using Epic.Server.Services;
using FrameworkSDK.DependencyInjection;
using JetBrains.Annotations;

namespace Epic.Server
{
    [UsedImplicitly]
    public class ServerServicesModule : IServicesModule
    {
        public void RegisterServices(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.RegisterType<IConsoleController, LoggerConsoleMessagesViewer>();
            
            serviceRegistrator.RegisterType<ISessionsService, DefaultSessionService>();
            serviceRegistrator.RegisterType<IUsersService, DefaultUsersService>();
            serviceRegistrator.RegisterType<IAuthorizationService, DefaultAuthorizationService>();
            serviceRegistrator.RegisterType<IUserUnitsService, DefaultUserUnitsService>();
            serviceRegistrator.RegisterType<IUnitTypesService, DefaultUnitTypesService>();
            serviceRegistrator.RegisterType<IBattleDefinitionsService, DefaultBattleDefinitionsService>();
            serviceRegistrator.RegisterType<IBattleUnitsService, DefaultBattleUnitsService>();
            serviceRegistrator.RegisterType<IBattlesService, DefaultBattlesService>();
            serviceRegistrator.RegisterType<IClientConnectionsService<WebSocket>, WebSocketClientConnectionsService>();
            serviceRegistrator.RegisterType<IBattleGameManagersService, DefaultBattleGameManagersService>();
            serviceRegistrator.RegisterType<IClientMessagesParserService, ClientMessagesParserService>();
            serviceRegistrator.RegisterType<IBattleConnectionsService, BattleConnectionsService>();
            serviceRegistrator.RegisterType<IBattlesCacheService, DefaultBattlesCacheService>();
            serviceRegistrator.RegisterType<IRewardsService, DefaultRewardsService>();
            serviceRegistrator.RegisterType<IBattleLogicFactory, BattleLogicFactory>();
            
            serviceRegistrator.RegisterType<ISessionsRepository, InMemorySessionsRepository>();
            serviceRegistrator.RegisterType<IUsersRepository, InMemoryUsersRepository>();
            serviceRegistrator.RegisterType<IUserUnitsRepository, InMemoryUserUnitsRepository>();
            serviceRegistrator.RegisterType<IUnitTypesRepository, InMemoryUnitTypesRepository>();
            serviceRegistrator.RegisterType<IBattleDefinitionsRepository, InMemoryBattleDefinitionsRepository>();
            serviceRegistrator.RegisterType<IBattlesRepository, InMemoryBattlesRepository>();
            serviceRegistrator.RegisterType<IBattleUnitsRepository, InMemoryBattleUnitsRepository>();
            serviceRegistrator.RegisterType<IRewardsRepository, InMemoryRewardsRepository>();
        }
    }
}