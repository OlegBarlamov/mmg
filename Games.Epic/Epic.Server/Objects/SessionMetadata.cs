namespace Epic.Server.Objects
{
    public class SessionMetadata
    {
        public IDeviceInfo DeviceInfo { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
    }
}