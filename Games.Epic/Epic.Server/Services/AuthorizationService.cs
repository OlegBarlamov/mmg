using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Epic.Core;
using Epic.Core.Services.Users;
using Epic.Server.Authentication;
using Epic.Server.Objects;
using JetBrains.Annotations;

namespace Epic.Server.Services
{
    public interface IAuthorizationService
    {
        Task<ISessionObject> BasicLoginAsync(string username, string password, SessionMetadata sessionMetadata);
        Task<ClaimsPrincipal> AuthorizeAsync(ISessionObject session);
    }

    [UsedImplicitly]
    class DefaultAuthorizationService : IAuthorizationService
    {
        [NotNull] private ISessionsService SessionsService { get; }
        [NotNull] private IUsersService UsersService { get; }

        public DefaultAuthorizationService([NotNull] ISessionsService sessionsService, [NotNull] IUsersService usersService)
        {
            SessionsService = sessionsService ?? throw new ArgumentNullException(nameof(sessionsService));
            UsersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
        }
        
        public async Task<ISessionObject> BasicLoginAsync(string username, string password, SessionMetadata sessionMetadata)
        {
            var basicHash = BasicAuthentication.GetHashFromCredentials(username, password);
            var userObject = await UsersService.GetUserByHashAsync(basicHash);
            return await SessionsService.CreateSession(userObject, sessionMetadata);
        }

        public async Task<ClaimsPrincipal> AuthorizeAsync(ISessionObject session)
        {
            await SessionsService.UpdateLastAccessed(session);
            
            var identity = new ClaimsIdentity(Array.Empty<Claim>(), "CookieAuth");
            identity.FillWithSessionData(session);
            
            return new ClaimsPrincipal(identity);
        }
    }
}