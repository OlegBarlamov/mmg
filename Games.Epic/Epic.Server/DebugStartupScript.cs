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

        private Guid _userId;
        
        public DebugStartupScript([NotNull] IUsersRepository usersRepository)
        {
            UsersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
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
            });
        }
    }
}