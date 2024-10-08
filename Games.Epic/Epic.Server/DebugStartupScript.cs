using System;
using System.Threading.Tasks;
using Epic.Core;
using Epic.Data;
using Epic.Data.BattleDefinitions;
using Epic.Data.UnitTypes;
using Epic.Data.UserUnits;
using Epic.Server.Authentication;
using FrameworkSDK;
using JetBrains.Annotations;

namespace Epic.Server
{
    public class DebugStartupScript : IAppComponent
    {
        [NotNull] public IUsersService UsersService { get; }
        [NotNull] public IUserUnitsRepository UserUnitsRepository { get; }
        [NotNull] public IUsersRepository UsersRepository { get; }
        [NotNull] public ISessionsRepository SessionsRepository { get; }
        public IBattleDefinitionsRepository BattleDefinitionsRepository { get; }
        [NotNull] public IUnitTypesRepository UnitTypesRepository { get; set; }

        private Guid _userId;
        
        public DebugStartupScript(
            [NotNull] IUsersRepository usersRepository,
            [NotNull] ISessionsRepository sessionsRepository,
            [NotNull] IBattleDefinitionsRepository battleDefinitionsRepository,
            [NotNull] IUnitTypesRepository unitTypesRepository,
            [NotNull] IUsersService usersService,
            [NotNull] IUserUnitsRepository userUnitsRepository)
        {
            UsersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
            UserUnitsRepository = userUnitsRepository ?? throw new ArgumentNullException(nameof(userUnitsRepository));
            UsersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
            SessionsRepository = sessionsRepository ?? throw new ArgumentNullException(nameof(sessionsRepository));
            BattleDefinitionsRepository = battleDefinitionsRepository ?? throw new ArgumentNullException(nameof(battleDefinitionsRepository));
            UnitTypesRepository = unitTypesRepository ?? throw new ArgumentNullException(nameof(unitTypesRepository));
        }
        
        public void Dispose()
        {
            UsersRepository.DeleteUserAsync(_userId);
        }

        public void Configure()
        {
            var unitTypeId = Guid.NewGuid();
            UnitTypesRepository.CreateUnitType(unitTypeId, new UnitTypeProperties
            {
                Speed = 5,
                AttackMaxRange = 1,
                AttackMinRange = 1,
                Damage = 6,
                Health = 10,
                BattleImgUrl = "https://blz-contentstack-images.akamaized.net/v3/assets/blt0e00eb71333df64e/blt7c29bfc026dc8ab3/6606072a2c8f660cca84835a/human_icon_default.webp",
                DashboardImgUrl = "https://blz-contentstack-images.akamaized.net/v3/assets/blt0e00eb71333df64e/blt7c29bfc026dc8ab3/6606072a2c8f660cca84835a/human_icon_default.webp"
            });

            var computerUserTask = UsersService.CreateComputerUser();
            var computerUnit1 = computerUserTask.ContinueWith(task => UserUnitsRepository.CreateUserUnit(unitTypeId, 10, task.Result.Id, true));
            var computerUnit2 = computerUserTask.ContinueWith(task => UserUnitsRepository.CreateUserUnit(unitTypeId, 20, task.Result.Id, true));
            
            UsersRepository.CreateUserAsync("admin", BasicAuthentication.GetHashFromCredentials("admin", "123"),
                UserEntityType.Player).ContinueWith(task =>
            {
                var user = task.Result;
                _userId = user.Id;

                SessionsRepository.CreateSessionAsync("test_token", _userId, new SessionData());

                Task.WhenAll(computerUnit1, computerUnit2).ContinueWith(unitsTask =>
                {
                    var unitsTasks = unitsTask.Result;
                    BattleDefinitionsRepository.CreateBattleDefinitionAsync(_userId, 10, 8, new []{unitsTasks[0].Result.Id});
                    BattleDefinitionsRepository.CreateBattleDefinitionAsync(_userId, 6, 6, new []{unitsTasks[1].Result.Id});
                });
                
                UserUnitsRepository.CreateUserUnit(unitTypeId, 30, _userId, true);
            });
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
            public string BattleImgUrl { get; set; }
            public string DashboardImgUrl { get; set; }
        }
    }
}