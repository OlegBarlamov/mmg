using System;

namespace Epic.Data
{
    public interface ISessionData
    {
        DateTime Created { get; }
        DateTime LastAccessed { get; }
        DateTime? RevokedDate { get; }
        bool IsRevoked { get; }
        string RevokedReason { get; }
        string DeviceInfo { get; }
        string IpAddress { get; }
        string UserAgent { get; }
    }

    internal class SessionData : ISessionData
    {
        public DateTime Created { get; set; }
        public DateTime LastAccessed { get; set; }
        public DateTime? RevokedDate { get; set; }
        public bool IsRevoked { get; set;  }
        public string RevokedReason { get; set; }
        public string DeviceInfo { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
    }
}