namespace Epic.Server.Objects
{
    public class MutableDeviceInfo : IDeviceInfo
    {
        public bool IsMobile { get; set; }
        public bool IsTablet { get; set; }
        public bool IsDesktop { get; set; }
        public string Browser { get; set; }
        public string BrowserVersion { get; set; }
        public string OperatingSystem { get; set; }
        public string OperatingSystemVersion { get; set; }
        public string UserAgent { get; set; }
    }
}