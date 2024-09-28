namespace Epic.Server.Objects
{
    public interface IDeviceInfo
    {
        bool IsMobile { get; }
        bool IsTablet { get; }
        bool IsDesktop { get; }
        string Browser { get; }
        string BrowserVersion { get; }
        string OperatingSystem { get; }
        string OperatingSystemVersion { get; }
        string UserAgent { get; }
    }
}