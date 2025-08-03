using System;
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
using Epic.Server.Authentication;
using FrameworkSDK;
using JetBrains.Annotations;

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
            [NotNull] IGameResourcesRepository resourcesRepository)
        {
            UsersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
            GlobalUnitsRepository = globalUnitsRepository ?? throw new ArgumentNullException(nameof(globalUnitsRepository));
            RewardsRepository = rewardsRepository ?? throw new ArgumentNullException(nameof(rewardsRepository));
            PlayersService = playersService ?? throw new ArgumentNullException(nameof(playersService));
            UnitsContainersService = unitsContainersService ?? throw new ArgumentNullException(nameof(unitsContainersService));
            BattleDefinitionsService = battleDefinitionsService ?? throw new ArgumentNullException(nameof(battleDefinitionsService));
            HeroesService = heroesService ?? throw new ArgumentNullException(nameof(heroesService));
            ResourcesRepository = resourcesRepository ?? throw new ArgumentNullException(nameof(resourcesRepository));
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
            
            var user = await UsersRepository.CreateUserAsync("admin",
                BasicAuthentication.GetHashFromCredentials("admin", "123"));
            await SessionsRepository.CreateSessionAsync("test_token", user.Id, new SessionData());
            
            var userPlayer = await PlayersService.CreatePlayer(user.Id, "admin_player", PlayerObjectType.Human);

            await ResourcesRepository.GiveResource(ResourcesRepository.GoldResourceId, userPlayer.Id, 5000);
            
            var hero = await HeroesService.CreateNew(userPlayer.Name, userPlayer.Id);
            await PlayersService.SetActiveHero(userPlayer.Id, hero.Id);
            
            await GlobalUnitsRepository.Create(pickerUnitType.Id, 30, hero.ArmyContainerId, true, 0);
            await GlobalUnitsRepository.Create(archerUnitType.Id, 20, hero.ArmyContainerId, true, 1);
            
            
            var bd1 = await BattleDefinitionsService.CreateBattleDefinition(userPlayer.Id, 10, 8, 3);
            var bd2 = await BattleDefinitionsService.CreateBattleDefinition(userPlayer.Id, 6, 6, 3);
            var bd3 = await BattleDefinitionsService.CreateBattleDefinition(userPlayer.Id, 7, 7, 3);
            
            await GlobalUnitsRepository.Create(pickerUnitType.Id, 10, bd1.ContainerId, true, 0);
            await GlobalUnitsRepository.Create(pickerUnitType.Id, 20, bd2.ContainerId, true, 0);
            await GlobalUnitsRepository.Create(archerUnitType.Id, 30, bd3.ContainerId, true, 0);

            await RewardsRepository.CreateRewardAsync(bd1.Id, new MutableRewardFields
                {
                    Amounts  = new[] { 5000 },
                    CanDecline = true,
                    Ids = new[] { ResourcesRepository.GoldResourceId },
                    RewardType = RewardType.ResourcesGain,
                    Message = "Reward!", 
                }
            );
            await RewardsRepository.CreateRewardAsync(bd2.Id, new MutableRewardFields
                {
                  Amounts  = new[] { 10 },
                  CanDecline = true,
                  Ids = new[] { pickerUnitType.Id },
                  RewardType = RewardType.UnitsGain,
                  Message = "Reward!", 
                }
            );
            
            var bd3_1 = await BattleDefinitionsService.CreateBattleDefinition(6, 6);
            await GlobalUnitsRepository.Create(archerUnitType.Id, 15, bd3_1.ContainerId, true, 0);
            await GlobalUnitsRepository.Create(pickerUnitType.Id, 20, bd3_1.ContainerId, true, 1);
            await GlobalUnitsRepository.Create(archerUnitType.Id, 15, bd3_1.ContainerId, true, 2);
            await RewardsRepository.CreateRewardAsync(bd3_1.Id, new MutableRewardFields
                {
                    Amounts  = new []{ 100, 50 },
                    CanDecline = true,
                    Ids = new []{pickerUnitType.Id, archerUnitType.Id},
                    RewardType = RewardType.UnitToBuy,
                    Message = "Now you can train units",
                    CustomIconUrl = "https://heroes.thelazy.net/images/6/63/Adventure_Map_Castle_capitol.gif",
                    CustomTitle = "Units dwelling",
                }
            );
            
            await RewardsRepository.CreateRewardAsync(bd3.Id, new MutableRewardFields
                {
                    Amounts  = Array.Empty<int>(),
                    CanDecline = false,
                    Ids = Array.Empty<Guid>(),
                    RewardType = RewardType.Battle,
                    Message = "You need to defeat the guard.",
                    CustomIconUrl = "https://heroes.thelazy.net/images/6/63/Adventure_Map_Castle_capitol.gif",
                    CustomTitle = "Units dwelling",
                    NextBattleDefinitionId = bd3_1.Id,
                }
            );
            
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