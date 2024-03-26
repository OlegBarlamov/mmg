namespace FrameworkSDK.MonoGame.Services
{
    public interface IAppStateService
    {
        bool IsRunning { get; }
        bool IsTerminating { get; }
        bool IsInitialized { get; }
        bool IsInitializing { get; }
        bool CoreResourceLoaded { get; }
        bool CoreResourceLoading { get; }
        bool CoreResourceUnloading { get; set; }
        bool IsUpdateStateActive { get; }
        bool IsDrawStateActive { get; }
        bool IsAppFocused { get; }
    }
}