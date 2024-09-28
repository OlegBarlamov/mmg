using System;

namespace Epic.Data
{
    public interface ISessionEntity
    {
        Guid Id { get; }
        string Token { get; }
        Guid UserId { get; }
        ISessionData Data { get; }
    }
    
    internal class SessionEntity : ISessionEntity
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
        public Guid UserId { get; set; }
        
        public SessionData Data { get; set; }
        ISessionData ISessionEntity.Data => Data;
    }
}