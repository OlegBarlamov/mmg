using FrameworkSDK.DependencyInjection;
using FrameworkSDK.Services;

namespace FrameworkSDK.MonoGame.Services
{
    public static class DebugServicesOnlyForDebug
    {
        public static IDebugInfoService DebugInfoService => AppContext.ServiceLocator.Resolve<IDebugInfoService>();

        public static IDebugVariablesService DebugVariablesService =>
            AppContext.ServiceLocator.Resolve<IDebugVariablesService>();
    }
}