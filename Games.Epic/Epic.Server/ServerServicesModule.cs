using System.Net.WebSockets;
using Console.Core;
using Console.FrameworkAdapter;
using Epic.Core.Logic;
using Epic.Core.Services;
using Epic.Core.Services.BattleDefinitions;
using Epic.Core.Services.BattleObstacles;
using Epic.Core.Services.BattleReports;
using Epic.Core.Services.Battles;
using Epic.Core.Services.Connection;
using Epic.Core.Services.Buffs;
using Epic.Core.Services.BuffTypes;
using Epic.Core.Services.GameManagement;
using Epic.Core.Services.GameResources;
using Epic.Core.Services.Heroes;
using Epic.Core.Services.Players;
using Epic.Core.Services.RewardDefinitions;
using Epic.Core.Services.Rewards;
using Epic.Core.Services.Artifacts;
using Epic.Core.Services.ArtifactTypes;
using Epic.Core.Services.Units;
using Epic.Core.Services.UnitsContainers;
using Epic.Core.Services.UnitTypes;
using Epic.Core.Services.Users;
using Epic.Data;
using Epic.Data.Artifact;
using Epic.Data.ArtifactType;
using Epic.Data.Buff;
using Epic.Data.BuffType;
using Epic.Data.BattleDefinitions;
using Epic.Data.BattleObstacles;
using Epic.Data.BattleReports;
using Epic.Data.Battles;
using Epic.Data.BattleUnits;
using Epic.Data.GameResources;
using Epic.Data.GlobalUnits;
using Epic.Data.Heroes;
using Epic.Data.Players;
using Epic.Data.Reward;
using Epic.Data.RewardDefinitions;
using Epic.Data.UnitsContainers;
using Epic.Data.UnitTypes;
using Epic.Logic.Battle;
using Epic.Logic.Battle.Obstacles;
using Epic.Logic.Generator;
using Epic.Logic.Heroes;
using Epic.Logic.Rewards;
using Epic.Server.Services;
using FrameworkSDK.Common;
using FrameworkSDK.DependencyInjection;
using FrameworkSDK.Services.Randoms;
using JetBrains.Annotations;
using DefaultBattleUnitsService = Epic.Core.Services.Battles.DefaultBattleUnitsService;

namespace Epic.Server
{
    [UsedImplicitly]
    public class ServerServicesModule : IServicesModule
    {
        public void RegisterServices(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.RegisterInstance<IRandomSeedProvider>(new FixedSeedProvider());
            serviceRegistrator.RegisterType<IRandomService, DefaultRandomService>();
            
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
            serviceRegistrator.RegisterType<IArtifactsService, DefaultArtifactsService>();
            serviceRegistrator.RegisterType<IArtifactTypesService, DefaultArtifactTypesService>();
            serviceRegistrator.RegisterType<IBuffTypesService, DefaultBuffTypesService>();
            serviceRegistrator.RegisterType<IBuffsService, DefaultBuffsService>();
            serviceRegistrator.RegisterType<IGameResourcesService, DefaultGameResourcesService>();
            serviceRegistrator.RegisterType<IBattleReportsService, DefaultBattleReportsService>();
            serviceRegistrator.RegisterType<DefaultUnitTypesRegistry, DefaultUnitTypesRegistry>();
            serviceRegistrator.RegisterFactory<IUnitTypesRegistry>((locator, _) => locator.Resolve<DefaultUnitTypesRegistry>());
            serviceRegistrator.RegisterType<DefaultArtifactTypesRegistry, DefaultArtifactTypesRegistry>();
            serviceRegistrator.RegisterFactory<IArtifactTypesRegistry>((locator, _) => locator.Resolve<DefaultArtifactTypesRegistry>());
            serviceRegistrator.RegisterType<DefaultBuffTypesRegistry, DefaultBuffTypesRegistry>();
            serviceRegistrator.RegisterFactory<IBuffTypesRegistry>((locator, _) => locator.Resolve<DefaultBuffTypesRegistry>());
            serviceRegistrator.RegisterType<DefaultGameResourcesRegistry, DefaultGameResourcesRegistry>();
            serviceRegistrator.RegisterFactory<IGameResourcesRegistry>((locator, _) => locator.Resolve<DefaultGameResourcesRegistry>());
            serviceRegistrator.RegisterType<IBattleObstaclesService, DefaultBattleObstaclesService>();
            serviceRegistrator.RegisterType<DefaultRewardDefinitionsRegistry, DefaultRewardDefinitionsRegistry>();
            serviceRegistrator.RegisterFactory<IRewardDefinitionsRegistry>((locator, _) => locator.Resolve<DefaultRewardDefinitionsRegistry>());
            serviceRegistrator.RegisterType<FixedGameModeProvider, FixedGameModeProvider>();
            serviceRegistrator.RegisterFactory<IGameModeProvider>((locator, _) => locator.Resolve<FixedGameModeProvider>());
            
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
            serviceRegistrator.RegisterType<IBattleObstaclesRepository, InMemoryBattleObstaclesRepository>();
            serviceRegistrator.RegisterType<IRewardDefinitionsRepository, InMemoryRewardDefinitionsRepository>();
            serviceRegistrator.RegisterType<IArtifactsRepository, InMemoryArtifactsRepository>();
            serviceRegistrator.RegisterType<IArtifactTypesRepository, InMemoryArtifactTypesRepository>();
            serviceRegistrator.RegisterType<IBuffTypesRepository, InMemoryBuffTypesRepository>();
            serviceRegistrator.RegisterType<IBuffsRepository, InMemoryBuffsRepository>();
            
            serviceRegistrator.RegisterType<IBattleLogicFactory, BattleLogicFactory>();
            serviceRegistrator.RegisterType<IDaysProcessor, DaysProcessor>();
            serviceRegistrator.RegisterType<IBattlesGenerator, BattleGenerator>();
            serviceRegistrator.RegisterType<IBattleUnitsPlacer, BattleUnitsPlacer>();
            serviceRegistrator.RegisterType<IHeroesLevelsCalculator, HeroesLevelsCalculator>();
            serviceRegistrator.RegisterType<IBattleObstaclesGenerator, BattleObstaclesGenerator>();
            serviceRegistrator.RegisterType<IRewardDefinitionsService, DefaultRewardDefinitionsService>();
            serviceRegistrator.RegisterType<GlobalUnitsForBattleGenerator, GlobalUnitsForBattleGenerator>();

            PredefinedStaticResources.QuestionIconUrl = "/resources/question.png";
        }
    }
}