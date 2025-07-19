using System.Net.WebSockets;
using Console.Core;
using Console.FrameworkAdapter;
using Epic.Core.Services.BattleDefinitions;
using Epic.Core.Services.Battles;
using Epic.Core.Services.Connection;
using Epic.Core.Services.GameManagement;
using Epic.Core.Services.Players;
using Epic.Core.Services.Rewards;
using Epic.Core.Services.Units;
using Epic.Core.Services.UnitTypes;
using Epic.Core.Services.Users;
using Epic.Data;
using Epic.Data.BattleDefinitions;
using Epic.Data.Battles;
using Epic.Data.BattleUnits;
using Epic.Data.Players;
using Epic.Data.PlayerUnits;
using Epic.Data.Reward;
using Epic.Data.UnitTypes;
using Epic.Logic;
using Epic.Server.Services;
using FrameworkSDK.DependencyInjection;
using JetBrains.Annotations;
using DefaultBattleUnitsService = Epic.Core.Services.Battles.DefaultBattleUnitsService;

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
            serviceRegistrator.RegisterType<IPlayerUnitsService, DefaultPlayerUnitsService>();
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
            serviceRegistrator.RegisterType<IPlayersService, DefaultPlayersService>();
            
            serviceRegistrator.RegisterType<ISessionsRepository, InMemorySessionsRepository>();
            serviceRegistrator.RegisterType<IUsersRepository, InMemoryUsersRepository>();
            serviceRegistrator.RegisterType<IPlayerUnitsRepository, InMemoryPlayerUnitsRepository>();
            serviceRegistrator.RegisterType<IUnitTypesRepository, InMemoryUnitTypesRepository>();
            serviceRegistrator.RegisterType<IBattleDefinitionsRepository, InMemoryBattleDefinitionsRepository>();
            serviceRegistrator.RegisterType<IBattlesRepository, InMemoryBattlesRepository>();
            serviceRegistrator.RegisterType<IBattleUnitsRepository, InMemoryBattleUnitsRepository>();
            serviceRegistrator.RegisterType<IRewardsRepository, InMemoryRewardsRepository>();
            serviceRegistrator.RegisterType<IPlayersRepository, InMemoryPlayersRepository>();
        }
    }
}