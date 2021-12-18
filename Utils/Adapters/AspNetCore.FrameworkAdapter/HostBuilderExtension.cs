using System;
using FrameworkSDK;
using FrameworkSDK.Constructing;
using FrameworkSDK.Logging;
using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;

namespace AspNetCore.FrameworkAdapter
{
    public static class HostBuilderExtension
    {
        public static IHostBuilder UseFramework([NotNull] this IHostBuilder hostBuilder, [CanBeNull] IFrameworkLogger frameworkLogger)
        {
            if (hostBuilder == null) throw new ArgumentNullException(nameof(hostBuilder));
            
            var appConfigurator = new AppFactory(frameworkLogger).Create()
                .SetupCustomLoggerIfNotNull(frameworkLogger)
        }
    }
}