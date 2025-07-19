using System;
using Epic.Core.Services.Players;
using Epic.Core.Services.Users;
using Epic.Data;
using Epic.Data.BattleDefinitions;
using Epic.Data.PlayerUnits;
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
        [NotNull] public IPlayerUnitsRepository PlayerUnitsRepository { get; }
        public IRewardsRepository RewardsRepository { get; }
        public IPlayersService PlayersService { get; }
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
            [NotNull] IPlayersService playersService)
        {
            UsersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
            PlayerUnitsRepository = playerUnitsRepository ?? throw new ArgumentNullException(nameof(playerUnitsRepository));
            RewardsRepository = rewardsRepository ?? throw new ArgumentNullException(nameof(rewardsRepository));
            PlayersService = playersService ?? throw new ArgumentNullException(nameof(playersService));
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
                Speed = 5,
                AttackMaxRange = 1,
                AttackMinRange = 1,
                Damage = 6,
                Health = 10,
                BattleImgUrl =
                    "https://blz-contentstack-images.akamaized.net/v3/assets/blt0e00eb71333df64e/blt7c29bfc026dc8ab3/6606072a2c8f660cca84835a/human_icon_default.webp",
                DashboardImgUrl =
                    "https://blz-contentstack-images.akamaized.net/v3/assets/blt0e00eb71333df64e/blt7c29bfc026dc8ab3/6606072a2c8f660cca84835a/human_icon_default.webp"
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

            var computerUnit1 = await PlayerUnitsRepository.CreatePlayerUnit(unitTypeId, 10, computerPlayerId, true);
            var computerUnit2 = await PlayerUnitsRepository.CreatePlayerUnit(unitTypeId, 20, computerPlayerId, true);
            
            var bd1 = await BattleDefinitionsRepository.Create(userPlayerId, 10, 8,
                new[] { computerUnit1.Id });
            var bd2 = await BattleDefinitionsRepository.Create(userPlayerId, 6, 6,
                new[] { computerUnit2.Id });


            await RewardsRepository.CreateRewardAsync(bd2.Id, RewardType.UnitsGain,
                new[] { unitTypeId }, new[] { 10 }, "Reward!");

            await PlayerUnitsRepository.CreatePlayerUnit(unitTypeId, 30, userPlayerId, true);
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

        private class UnitTypeProperties : IUnitTypeProperties
        {
            public int Speed { get; set; }
            public int AttackMaxRange { get; set; }
            public int AttackMinRange { get; set; }
            public int Damage { get; set; }
            public int Health { get; set; }
            public string Name { get; set; }
            public string BattleImgUrl { get; set; }
            public string DashboardImgUrl { get; set; }
        }
    }
}