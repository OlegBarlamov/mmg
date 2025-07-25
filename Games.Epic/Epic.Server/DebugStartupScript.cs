using System;
using System.Collections.Generic;
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
using Epic.Data.UnitTypes.Subtypes;
using Epic.Data.UnitTypes.Subtypes.Presets;
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
            var unitTypeId = Guid.NewGuid();
            await UnitTypesRepository.CreateUnitType(unitTypeId, new UnitTypeProperties
            {
                Name = "Unit",
                Health = 10,
                Speed = 5,
                Value = 100,
                Attacks = new List<AttackFunctionType>
                {
                    new MeleeAttackType
                    {
                        MinDamage = 4,
                        MaxDamage = 6,
                    },
                },
                BattleImgUrl =
                    "https://blz-contentstack-images.akamaized.net/v3/assets/blt0e00eb71333df64e/blt7c29bfc026dc8ab3/6606072a2c8f660cca84835a/human_icon_default.webp",
                DashboardImgUrl =
                    "https://blz-contentstack-images.akamaized.net/v3/assets/blt0e00eb71333df64e/blt7c29bfc026dc8ab3/6606072a2c8f660cca84835a/human_icon_default.webp"
            });
            var archerTypeId = Guid.NewGuid();
            await UnitTypesRepository.CreateUnitType(archerTypeId, new UnitTypeProperties
            {
                Name = "Archer",
                Health = 7,
                Speed = 4,
                Value = 70,
                Attacks = new List<AttackFunctionType>
                {
                    new RangeAttackType
                    {
                        MinDamage = 1,
                        MaxDamage = 5,
                        AttackMaxRange = 7,
                        AttackMinRange = 3,
                    },
                    new MeleeAttackType
                    {
                        MinDamage = 1,
                        MaxDamage = 2,
                    },
                },
                BattleImgUrl =
                    "https://pbs.twimg.com/media/FkWlZU0XkAEUFdN.png",
                DashboardImgUrl =
                    "https://pbs.twimg.com/media/FkWlZU0XkAEUFdN.png"
            });
            
            
            
            
            var user = await UsersRepository.CreateUserAsync("admin",
                BasicAuthentication.GetHashFromCredentials("admin", "123"));
            await SessionsRepository.CreateSessionAsync("test_token", user.Id, new SessionData());
            
            var userPlayer = await PlayersService.CreatePlayer(user.Id, "admin_player", PlayerObjectType.Human);

            await ResourcesRepository.GiveResource(ResourcesRepository.GoldResourceId, userPlayer.Id, 5000);
            
            var hero = await HeroesService.CreateNew(userPlayer.Name, userPlayer.Id);
            await PlayersService.SetActiveHero(userPlayer.Id, hero.Id);
            
            await GlobalUnitsRepository.Create(unitTypeId, 30, hero.ArmyContainerId, true, 0);
            await GlobalUnitsRepository.Create(archerTypeId, 20, hero.ArmyContainerId, true, 1);
            
            
            
            
            
            var bd1 = await BattleDefinitionsService.CreateBattleDefinition(userPlayer.Id, 10, 8);
            var bd2 = await BattleDefinitionsService.CreateBattleDefinition(userPlayer.Id, 6, 6);
            var bd3 = await BattleDefinitionsService.CreateBattleDefinition(userPlayer.Id, 7, 7);
            
            await GlobalUnitsRepository.Create(unitTypeId, 10, bd1.ContainerId, true, 0);
            await GlobalUnitsRepository.Create(unitTypeId, 20, bd2.ContainerId, true, 0);
            await GlobalUnitsRepository.Create(archerTypeId, 30, bd3.ContainerId, true, 0);

            await RewardsRepository.CreateRewardAsync(bd2.Id, RewardType.UnitsGain,
                new[] { unitTypeId }, new[] { 10 }, "Reward!");
            await RewardsRepository.CreateRewardAsync(bd1.Id, RewardType.ResourcesGain,
                new[] { ResourcesRepository.GoldResourceId }, new[] { 5000 }, "Reward!");
            await RewardsRepository.CreateRewardAsync(bd3.Id, RewardType.UnitToBuy,
                new[] { unitTypeId, archerTypeId }, new[] { 100, 50 }, "Dwelling");
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