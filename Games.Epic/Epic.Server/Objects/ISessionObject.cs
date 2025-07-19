using System;

namespace Epic.Server.Objects
{
    public interface ISessionObject
    {
        Guid Id { get; }
        string Token { get; }
        Guid UserId { get; }
        Guid? PlayerId { get; }
        
        DateTime Created { get; }
        DateTime LastAccessed { get; }
        bool IsRevoked { get; }
        string DeviceInfo { get; }
        string IpAddress { get; }
        string UserAgent { get; }
    }
}