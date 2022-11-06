using FrameworkSDK;

namespace AspNetCore.FrameworkAdapter
{
    public static class AppExtensions
    {
        public static IAspNetCoreApp Asp(this IApp app)
        {
            return (IAspNetCoreApp)app;
        }
    }
}