using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Epic.Core.Objects;
using Epic.Core.Services.Players;
using Epic.Core.Services.Users;
using Epic.Data;
using Epic.Data.Exceptions;
using Epic.Server.Exceptions;
using Epic.Server.Objects;
using JetBrains.Annotations;

namespace Epic.Server.Services
{
    public interface ISessionsService
    {
        Task<ITokenValidationResult> ValidateToken(string token);
        Task UpdateLastAccessed(ISessionObject sessionObject);
        Task SetPlayerId(ISessionObject sessionObject, Guid? playerId);
        Task RevokeSession(ISessionObject sessionObject, SessionRevokedReason reason);
        Task<ISessionObject> CreateSession(IUserObject user, SessionMetadata metadata);
        Task<ISessionObject> GetSessionByToken(string token);
    }

    [UsedImplicitly]
    class DefaultSessionService : ISessionsService
    {
        public ISessionsRepository SessionsRepository { get; }
        public IPlayersService PlayersService { get; }

        public DefaultSessionService([NotNull] ISessionsRepository sessionsRepository, [NotNull] IPlayersService playersService)
        {
            SessionsRepository = sessionsRepository ?? throw new ArgumentNullException(nameof(sessionsRepository));
            PlayersService = playersService ?? throw new ArgumentNullException(nameof(playersService));
        }
        
        public async Task<ITokenValidationResult> ValidateToken(string token)
        {
            try
            {
                var sessionEntity = await SessionsRepository.GetSessionByTokenAsync(token);
                var sessionObject = ToSessionObject(sessionEntity);
                if (sessionObject.IsRevoked)
                {
                    return new TokenRevokedValidationResult(sessionObject);
                }

                return new TokenValidValidationResult(sessionObject);
            }
            catch (EntityNotFoundException)
            {
                return new TokenInvalidValidationResult();
            }
        }

        public Task UpdateLastAccessed(ISessionObject sessionObject)
        {
            return SessionsRepository.UpdateLastVisit(sessionObject.Id, DateTime.Now);
        }

        public async Task SetPlayerId(ISessionObject sessionObject, Guid? playerId)
        {
            if (playerId.HasValue)
            {
                var player = await PlayersService.GetById(playerId.Value);
                if (player.UserId != sessionObject.UserId)
                    throw new InvalidOperationException("The player does not belong to this user.");
            }
            
            await SessionsRepository.SetPlayerId(sessionObject.Id, playerId);
        }

        public Task RevokeSession(ISessionObject sessionObject, SessionRevokedReason reason)
        {
            string reasonEntity = reason switch
            {
                SessionRevokedReason.Logout => "logout",
                SessionRevokedReason.Expired => "expired",
                _ => throw new ArgumentOutOfRangeException(nameof(reason), reason, null)
            };
            return SessionsRepository.SetRevokedAsync(sessionObject.Id, true, reasonEntity, DateTime.Now);
        }

        public async Task<ISessionObject> CreateSession(IUserObject user, SessionMetadata metadata)
        {
            if (user.IsBlocked)
                throw new UserBlockedException(user);
            if (user.IsSystem)
                throw new InvalidUserTypeException(user);

            var token = GenerateSecureToken(16);
            var sessionEntity = await SessionsRepository.CreateSessionAsync(token, user.Id, new MutableSessionData
            {
                Created = DateTime.Now,
                LastAccessed = DateTime.Now,
                DeviceInfo = GetDeviceInfoString(metadata.DeviceInfo),
                IsRevoked = false,
                IpAddress = metadata.IpAddress,
                RevokedDate = null,
                RevokedReason = null,
                UserAgent = metadata.UserAgent,
            });
            return ToSessionObject(sessionEntity);
        }

        public async Task<ISessionObject> GetSessionByToken(String token)
        {
            var sessionEntity = await SessionsRepository.GetSessionByTokenAsync(token);
            return ToSessionObject(sessionEntity);
        }

        private string GetDeviceInfoString(IDeviceInfo deviceInfo)
        {
            var deviceTypes = new List<string>();

            if (deviceInfo.IsMobile) deviceTypes.Add("Mobile");
            if (deviceInfo.IsTablet) deviceTypes.Add("Tablet");
            if (deviceInfo.IsDesktop) deviceTypes.Add("Desktop");
            
            string devices = string.Join(", ", deviceTypes);

            return $"{devices} | Browser: {deviceInfo.Browser} {deviceInfo.BrowserVersion} | OS: {deviceInfo.OperatingSystem} {deviceInfo.OperatingSystemVersion} | User Agent: {deviceInfo.UserAgent}";
        }

        public static ISessionObject ToSessionObject([NotNull] ISessionEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            return new MutableSessionObject
            {
                Id = entity.Id,
                Token = entity.Token,
                UserId = entity.UserId,
                PlayerId = entity.PlayerId,

                Created = entity.Data.Created,
                LastAccessed = entity.Data.LastAccessed,
                IsRevoked = entity.Data.IsRevoked,
                RevokedDate = entity.Data.RevokedDate,
                RevokedReason = entity.Data.RevokedReason,
                DeviceInfo = entity.Data.DeviceInfo,
                IpAddress = entity.Data.IpAddress,
                UserAgent = entity.Data.UserAgent,
            };
        }
        
        public static string GenerateSecureToken(int size = 32)
        {
            var tokenData = new byte[size];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(tokenData);
            }
            return Convert.ToBase64String(tokenData)
                .Replace("+", "") // Remove any '+' characters
                .Replace("/", "") // Remove any '/' characters
                .Replace("=", ""); // Remove any '=' characters
        }
        
        private class MutableSessionData : ISessionData 
        {
            public DateTime Created { get; set; }
            public DateTime LastAccessed { get; set; }
            public DateTime? RevokedDate { get; set; }
            public bool IsRevoked { get; set; }
            public string RevokedReason { get; set; }
            public string DeviceInfo { get; set; }
            public string IpAddress { get; set; }
            public string UserAgent { get; set; }
        }
    }
}