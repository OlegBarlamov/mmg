using System;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Services.BattleDefinitions;
using Epic.Core.Services.Heroes;
using Epic.Core.Services.Players;
using Epic.Core.Services.UnitsContainers;
using Epic.Core.Services.Users;
using Epic.Data;
using Epic.Data.BattleDefinitions;
using Epic.Data.GameResources;
using Epic.Data.GlobalUnits;
using Epic.Data.Reward;
using Epic.Data.UnitTypes;
using Epic.Logic;
using Epic.Logic.Generator;
using Epic.Server.Authentication;
using FrameworkSDK;
using JetBrains.Annotations;
using NetExtensions.Collections;

namespace Epic.Server
{
    public class DebugStartupScript : IAppComponent
    {
        [NotNull] public IUsersService UsersService { get; }
        [NotNull] public IGlobalUnitsRepository GlobalUnitsRepository { get; }
        public IRewardsRepository RewardsRepository { get; }
        public IPlayersService PlayersService { get; }
        public IUnitsContainersService UnitsContainersService { get; }
        public IBattleDefinitionsService BattleDefinitionsService { get; }
        public IHeroesService HeroesService { get; }
        public IGameResourcesRepository ResourcesRepository { get; }
        public IBattlesGenerator BattlesGenerator { get; }
        [NotNull] public IUsersRepository UsersRepository { get; }
        [NotNull] public ISessionsRepository SessionsRepository { get; }
        public IBattleDefinitionsRepository BattleDefinitionsRepository { get; }
        [NotNull] public IUnitTypesRepository UnitTypesRepository { get; set; }
        
        public DebugStartupScript(
            [NotNull] IUsersRepository usersRepository,
            [NotNull] ISessionsRepository sessionsRepository,
            [NotNull] IBattleDefinitionsRepository battleDefinitionsRepository,
            [NotNull] IUnitTypesRepository unitTypesRepository,
            [NotNull] IUsersService usersService,
            [NotNull] IGlobalUnitsRepository globalUnitsRepository,
            [NotNull] IRewardsRepository rewardsRepository,
            [NotNull] IPlayersService playersService,
            [NotNull] IUnitsContainersService unitsContainersService,
            [NotNull] IBattleDefinitionsService battleDefinitionsService,
            [NotNull] IHeroesService heroesService,
            [NotNull] IGameResourcesRepository resourcesRepository,
            [NotNull] IBattlesGenerator battlesGenerator)
        {
            UsersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
            GlobalUnitsRepository = globalUnitsRepository ?? throw new ArgumentNullException(nameof(globalUnitsRepository));
            RewardsRepository = rewardsRepository ?? throw new ArgumentNullException(nameof(rewardsRepository));
            PlayersService = playersService ?? throw new ArgumentNullException(nameof(playersService));
            UnitsContainersService = unitsContainersService ?? throw new ArgumentNullException(nameof(unitsContainersService));
            BattleDefinitionsService = battleDefinitionsService ?? throw new ArgumentNullException(nameof(battleDefinitionsService));
            HeroesService = heroesService ?? throw new ArgumentNullException(nameof(heroesService));
            ResourcesRepository = resourcesRepository ?? throw new ArgumentNullException(nameof(resourcesRepository));
            BattlesGenerator = battlesGenerator ?? throw new ArgumentNullException(nameof(battlesGenerator));
            UsersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
            SessionsRepository = sessionsRepository ?? throw new ArgumentNullException(nameof(sessionsRepository));
            BattleDefinitionsRepository = battleDefinitionsRepository ?? throw new ArgumentNullException(nameof(battleDefinitionsRepository));
            UnitTypesRepository = unitTypesRepository ?? throw new ArgumentNullException(nameof(unitTypesRepository));
        }
        
        public void Dispose()
        {
        }

        public async void Configure()
        {
            var pickerUnitType = await UnitTypesRepository.GetByName("Pikeman");
            var archerUnitType = await UnitTypesRepository.GetByName("Archer");
            var archangelUnitType = await UnitTypesRepository.GetByName("Archangel");
            
            var user = await UsersRepository.CreateUserAsync("admin",
                BasicAuthentication.GetHashFromCredentials("admin", "123"));
            var user1 = await UsersRepository.CreateUserAsync("admin1",
                BasicAuthentication.GetHashFromCredentials("admin1", "123"));
            await SessionsRepository.CreateSessionAsync("test_token", user.Id, new SessionData());
            await SessionsRepository.CreateSessionAsync("test_token", user1.Id, new SessionData());
            
            var userPlayer = await PlayersService.CreatePlayer(user.Id, "admin_player", PlayerObjectType.Human);
            var user1Player = await PlayersService.CreatePlayer(user1.Id, "admin_1_player", PlayerObjectType.Human);

            await ResourcesRepository.GiveResource(ResourcesRepository.GoldResourceId, userPlayer.Id, 500);
            await ResourcesRepository.GiveResource(ResourcesRepository.GoldResourceId, user1Player.Id, 500);
            // var resourcesByKeys = await ResourcesRepository.GetAllResourcesByKeys();
            // await Task.WhenAll(resourcesByKeys.Values.Select(async x =>
            // {
            //     if (x.Id != ResourcesRepository.GoldResourceId)
            //     {
            //         await ResourcesRepository.GiveResource(x.Id, userPlayer.Id, 1);
            //         await ResourcesRepository.GiveResource(x.Id, user1Player.Id, 1);
            //     }
            // }));
            
            var hero = await HeroesService.CreateNew(userPlayer.Name, userPlayer.Id);
            await PlayersService.SetActiveHero(userPlayer.Id, hero.Id);
            
            var hero1 = await HeroesService.CreateNew(user1Player.Name, user1Player.Id);
            await PlayersService.SetActiveHero(user1Player.Id, hero1.Id);
            
            await GlobalUnitsRepository.Create(pickerUnitType.Id, 10, hero.ArmyContainerId, true, 0);
            await GlobalUnitsRepository.Create(archerUnitType.Id, 6, hero.ArmyContainerId, true, 1);
            // await GlobalUnitsRepository.Create(archangelUnitType.Id, 1, hero.ArmyContainerId, true, 2);
            
            await GlobalUnitsRepository.Create(pickerUnitType.Id, 10, hero1.ArmyContainerId, true, 0);
            await GlobalUnitsRepository.Create(archerUnitType.Id, 6, hero1.ArmyContainerId, true, 1);

            await BattlesGenerator.GenerateSingle(userPlayer.Id, userPlayer.Day);
            await BattlesGenerator.GenerateSingle(userPlayer.Id, userPlayer.Day);
            await BattlesGenerator.GenerateSingle(userPlayer.Id, userPlayer.Day);
            await BattlesGenerator.GenerateSingle(userPlayer.Id, userPlayer.Day);
            await BattlesGenerator.GenerateSingle(userPlayer.Id, userPlayer.Day);
            await BattlesGenerator.GenerateSingle(userPlayer.Id, userPlayer.Day);
            await BattlesGenerator.GenerateSingle(userPlayer.Id, userPlayer.Day);
            
            await BattlesGenerator.GenerateSingle(user1Player.Id, user1Player.Day);
            await BattlesGenerator.GenerateSingle(user1Player.Id, user1Player.Day);
            await BattlesGenerator.GenerateSingle(user1Player.Id, user1Player.Day);
            await BattlesGenerator.GenerateSingle(user1Player.Id, user1Player.Day);
            await BattlesGenerator.GenerateSingle(user1Player.Id, user1Player.Day);
            await BattlesGenerator.GenerateSingle(user1Player.Id, user1Player.Day);
            await BattlesGenerator.GenerateSingle(user1Player.Id, user1Player.Day);
        }

        private class SessionData : ISessionData
        {
            public DateTime Created { get; } = DateTime.Now;
            public DateTime LastAccessed { get; } = DateTime.Now;
            public DateTime? RevokedDate { get; }  = null;
            public bool IsRevoked { get; } = false;
            public string RevokedReason { get; } = null;
            public string DeviceInfo { get; } = null;
            public string IpAddress { get; } = null;
            public string UserAgent { get; } = null;
        }
    }
}