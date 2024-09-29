using System;
using Epic.Data;
using Epic.Server.Authentication;
using FrameworkSDK;
using JetBrains.Annotations;

namespace Epic.Server
{
    public class DebugStartupScript : IAppComponent
    {
        [NotNull] public IUsersRepository UsersRepository { get; }
        [NotNull] public ISessionsRepository SessionsRepository { get; }

        private Guid _userId;
        
        public DebugStartupScript([NotNull] IUsersRepository usersRepository, [NotNull] ISessionsRepository sessionsRepository)
        {
            UsersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
            SessionsRepository = sessionsRepository ?? throw new ArgumentNullException(nameof(sessionsRepository));
        }
        
        public void Dispose()
        {
            UsersRepository.DeleteUserAsync(_userId);
        }

        public void Configure()
        {
            UsersRepository.CreateUserAsync("admin", BasicAuthentication.GetHashFromCredentials("admin", "123"),
                UserEntityType.Player).ContinueWith(task =>
            {
                var user = task.Result;
                _userId = user.Id;

                SessionsRepository.CreateSessionAsync("test_token", _userId, new SessionData());
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
    }
}