using FrameworkSDK.DependencyInjection;

namespace FrameworkSDK.MonoGame.Services
{
    public static class DebugInfoServiceSingletonOnlyForDebug
    {
        public static IDebugInfoService DebugInfoService => AppContext.ServiceLocator.Resolve<IDebugInfoService>();
    }
}