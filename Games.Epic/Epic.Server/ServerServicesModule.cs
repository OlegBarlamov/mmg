using System;
using System.Net.WebSockets;
using Console.Core;
using Console.FrameworkAdapter;
using Epic.Core.Logic;
using Epic.Core.Services;
using Epic.Core.Services.BattleDefinitions;
using Epic.Core.Services.BattleReports;
using Epic.Core.Services.Battles;
using Epic.Core.Services.Connection;
using Epic.Core.Services.GameManagement;
using Epic.Core.Services.GameResources;
using Epic.Core.Services.Heroes;
using Epic.Core.Services.Players;
using Epic.Core.Services.Rewards;
using Epic.Core.Services.Units;
using Epic.Core.Services.UnitsContainers;
using Epic.Core.Services.UnitTypes;
using Epic.Core.Services.Users;
using Epic.Data;
using Epic.Data.BattleDefinitions;
using Epic.Data.BattleReports;
using Epic.Data.Battles;
using Epic.Data.BattleUnits;
using Epic.Data.GameResources;
using Epic.Data.GlobalUnits;
using Epic.Data.Heroes;
using Epic.Data.Players;
using Epic.Data.Reward;
using Epic.Data.UnitsContainers;
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
            serviceRegistrator.RegisterInstance<IRandomProvider>(new FixedRandomProvider(new Random(0)));
            
            serviceRegistrator.RegisterType<IConsoleController, LoggerConsoleMessagesViewer>();
            
            serviceRegistrator.RegisterType<ISessionsService, DefaultSessionService>();
            serviceRegistrator.RegisterType<IUsersService, DefaultUsersService>();
            serviceRegistrator.RegisterType<IAuthorizationService, DefaultAuthorizationService>();
            serviceRegistrator.RegisterType<IGlobalUnitsService, DefaultGlobalUnitsService>();
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
            serviceRegistrator.RegisterType<IPlayersService, DefaultPlayersService>();
            serviceRegistrator.RegisterType<IUnitsContainersService, DefaultUnitsContainerService>();
            serviceRegistrator.RegisterType<IContainersManipulator, DefaultContainersManipulator>();
            serviceRegistrator.RegisterType<IHeroesService, DefaultHeroesService>();
            serviceRegistrator.RegisterType<IGameResourcesService, DefaultGameResourcesService>();
            serviceRegistrator.RegisterType<IBattleReportsService, DefaultBattleReportsService>();
            
            serviceRegistrator.RegisterType<ISessionsRepository, InMemorySessionsRepository>();
            serviceRegistrator.RegisterType<IUsersRepository, InMemoryUsersRepository>();
            serviceRegistrator.RegisterType<IGlobalUnitsRepository, InMemoryGlobalUnitsRepository>();
            serviceRegistrator.RegisterType<IUnitTypesRepository, InMemoryUnitTypesRepository>();
            serviceRegistrator.RegisterType<IBattleDefinitionsRepository, InMemoryBattleDefinitionsRepository>();
            serviceRegistrator.RegisterType<IBattlesRepository, InMemoryBattlesRepository>();
            serviceRegistrator.RegisterType<IBattleUnitsRepository, InMemoryBattleUnitsRepository>();
            serviceRegistrator.RegisterType<IRewardsRepository, InMemoryRewardsRepository>();
            serviceRegistrator.RegisterType<IPlayersRepository, InMemoryPlayersRepository>();
            serviceRegistrator.RegisterType<IUnitsContainerRepository, InMemoryUnitsContainerRepository>();
            serviceRegistrator.RegisterType<IHeroEntitiesRepository, InMemoryHeroEntitiesRepository>();
            serviceRegistrator.RegisterType<IGameResourcesRepository, InMemoryGameResourcesRepository>();
            serviceRegistrator.RegisterType<IBattleReportsRepository, InMemoryBattleReportsRepository>();
            
            serviceRegistrator.RegisterType<IBattleLogicFactory, BattleLogicFactory>();
            serviceRegistrator.RegisterType<IDaysProcessor, DaysProcessor>();
            serviceRegistrator.RegisterType<IBattlesGenerator, BattleGenerator>();
            serviceRegistrator.RegisterType<IBattleUnitsPlacer, BattleUnitsPlacer>();
            serviceRegistrator.RegisterType<IHeroesLevelsCalculator, HeroesLevelsCalculator>();

            PredefinedStaticResources.QuestionIconUrl = "/resources/question.png";
        }
    }
}