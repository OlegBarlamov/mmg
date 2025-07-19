using System;
using System.Threading.Tasks;

namespace Epic.Data
{
    public interface ISessionsRepository : IRepository
    {
        Task<ISessionEntity> GetSessionByTokenAsync(string token);
        Task<ISessionEntity> CreateSessionAsync(string token, Guid userId, ISessionData data);
        Task UpdateIsRevokedAsync(Guid sessionId, bool isRevoked);
        Task SetRevokedAsync(Guid sessionId, bool isRevoked, string reason, DateTime? revokedAt);
        Task UpdateLastVisit(Guid sessionId, DateTime lastVisit);
        Task SetPlayerId(Guid sessionId, Guid? playerId);
        Task DeleteSessionByIdAsync(Guid sessionId);
    }
}