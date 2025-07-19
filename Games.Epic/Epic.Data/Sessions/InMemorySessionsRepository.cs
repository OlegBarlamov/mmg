using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Data.Exceptions;
using JetBrains.Annotations;

namespace Epic.Data
{
    [UsedImplicitly]
    public class InMemorySessionsRepository : ISessionsRepository
    {
        public string Name => nameof(InMemorySessionsRepository);
        public string EntityName => "Session";

        private readonly Dictionary<Guid, SessionEntity> _sessions = new Dictionary<Guid, SessionEntity>();

        public Task<ISessionEntity> GetSessionByTokenAsync(string token)
        {
            var session = _sessions.Values.FirstOrDefault(s => s.Token == token);
            if (session == null)
                throw new EntityNotFoundException(this, token);
            
            return Task.FromResult((ISessionEntity)session);
        }

        public Task<ISessionEntity> CreateSessionAsync(string token, Guid userId, ISessionData data)
        {
            var session = new SessionEntity
            {
                Id = Guid.NewGuid(),
                Token = token,
                UserId = userId,
                Data = new SessionData
                {
                    Created = data.Created,
                    DeviceInfo = data.DeviceInfo,
                    IpAddress = data.IpAddress,
                    IsRevoked = data.IsRevoked,
                    LastAccessed = data.LastAccessed,
                    RevokedDate = data.RevokedDate,
                    RevokedReason = data.RevokedReason,
                    UserAgent = data.UserAgent,
                }
            };

            _sessions[session.Id] = session;
            return Task.FromResult((ISessionEntity)session);
        }

        public Task UpdateIsRevokedAsync(Guid sessionId, bool isRevoked)
        {
            if (_sessions.TryGetValue(sessionId, out var session))
            {
                session.Data.IsRevoked = isRevoked;
            }
            else
                throw new EntityNotFoundException(this, sessionId.ToString());
            
            return Task.CompletedTask;
        }

        public Task SetRevokedAsync(Guid sessionId, bool isRevoked, string reason, DateTime? revokedAt)
        {
            if (_sessions.TryGetValue(sessionId, out var session))
            {
                session.Data.IsRevoked = isRevoked;
                session.Data.RevokedReason = reason;
                session.Data.RevokedDate = revokedAt;
            }
            else
                throw new EntityNotFoundException(this, sessionId.ToString());

            return Task.CompletedTask;
        }

        public Task UpdateLastVisit(Guid sessionId, DateTime lastVisit)
        {
            if (_sessions.TryGetValue(sessionId, out var session))
            {
                session.Data.LastAccessed = lastVisit;
            }
            else
                throw new EntityNotFoundException(this, sessionId.ToString());

            return Task.CompletedTask;
        }

        public Task SetPlayerId(Guid sessionId, Guid? playerId)
        {
            if (_sessions.TryGetValue(sessionId, out var session))
            {
                session.PlayerId = playerId;
            } 
            else 
                throw new EntityNotFoundException(this, sessionId.ToString());
            
            return Task.CompletedTask;
        }

        public Task DeleteSessionByIdAsync(Guid sessionId)
        {
            _sessions.Remove(sessionId);
            return Task.CompletedTask;
        }
    }
}