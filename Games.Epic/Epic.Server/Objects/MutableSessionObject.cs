using System;

namespace Epic.Server.Objects
{
    public class MutableSessionObject : ISessionObject
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
        public Guid UserId { get; set; }
        
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