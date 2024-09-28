using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Epic.Core;
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
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, session.UserId.ToString()),
                new Claim(ClaimTypes.Name, session.UserId.ToString()),
                
                new Claim("SessionId", session.Id.ToString()),
                new Claim("Token", session.Token),
                new Claim("Created", session.Created.ToString("O")),
                new Claim("LastAccessed", session.LastAccessed.ToString("O")),
                new Claim("IsRevoked", session.IsRevoked.ToString()),
                new Claim("DeviceInfo", session.DeviceInfo ?? "Unknown"),
                new Claim("IpAddress", session.IpAddress ?? "Unknown"),
                new Claim("UserAgent", session.UserAgent ?? "Unknown")
            };
            
            var identity = new ClaimsIdentity(claims, "CookieAuth");
            return new ClaimsPrincipal(identity);
        }
    }
}