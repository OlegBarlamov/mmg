using System;
using System.Collections.Generic;
using Epic.Core.Services.BattleDefinitions;
using Epic.Core.Services.Players;
using Epic.Core.Services.UnitsContainers;
using Epic.Core.Services.Users;
using Epic.Data;
using Epic.Data.BattleDefinitions;
using Epic.Data.PlayerUnits;
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
        [NotNull] public IPlayerUnitsRepository PlayerUnitsRepository { get; }
        public IRewardsRepository RewardsRepository { get; }
        public IPlayersService PlayersService { get; }
        public IUnitsContainersService UnitsContainersService { get; }
        public IBattleDefinitionsService BattleDefinitionsService { get; }
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
            [NotNull] IPlayerUnitsRepository playerUnitsRepository,
            [NotNull] IRewardsRepository rewardsRepository,
            [NotNull] IPlayersService playersService,
            [NotNull] IUnitsContainersService unitsContainersService,
            [NotNull] IBattleDefinitionsService battleDefinitionsService)
        {
            UsersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
            PlayerUnitsRepository = playerUnitsRepository ?? throw new ArgumentNullException(nameof(playerUnitsRepository));
            RewardsRepository = rewardsRepository ?? throw new ArgumentNullException(nameof(rewardsRepository));
            PlayersService = playersService ?? throw new ArgumentNullException(nameof(playersService));
            UnitsContainersService = unitsContainersService ?? throw new ArgumentNullException(nameof(unitsContainersService));
            BattleDefinitionsService = battleDefinitionsService ?? throw new ArgumentNullException(nameof(battleDefinitionsService));
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

            var computerUser = await UsersService.CreateComputerUser();
            var user = await UsersRepository.CreateUserAsync("admin",
                BasicAuthentication.GetHashFromCredentials("admin", "123"));

            var userId = user.Id;
            var userPlayer = await PlayersService.CreatePlayer(userId, "admin_player", PlayerObjectType.Human);
            var computerPlayer = await PlayersService.CreateComputerPlayer(computerUser, userPlayer.Id);
            var computerPlayerId = computerPlayer.Id;
            var userPlayerId = userPlayer.Id;

            await SessionsRepository.CreateSessionAsync("test_token", userId, new SessionData());

            var bd1 = await BattleDefinitionsService.CreateBattleDefinition(userPlayerId, 10, 8);
            var bd2 = await BattleDefinitionsService.CreateBattleDefinition(userPlayerId, 6, 6);
            
            var computerUnit1 = await PlayerUnitsRepository.CreatePlayerUnit(unitTypeId, 10, computerPlayerId, bd1.ContainerId, true, 0);
            var computerUnit2 = await PlayerUnitsRepository.CreatePlayerUnit(unitTypeId, 20, computerPlayerId, bd2.ContainerId, true, 0);

            await RewardsRepository.CreateRewardAsync(bd2.Id, RewardType.UnitsGain,
                new[] { unitTypeId }, new[] { 10 }, "Reward!");

            await PlayerUnitsRepository.CreatePlayerUnit(unitTypeId, 30, userPlayerId, userPlayer.ArmyContainerId, true, 0);
            await PlayerUnitsRepository.CreatePlayerUnit(archerTypeId, 20, userPlayerId, userPlayer.ArmyContainerId, true, 1);
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