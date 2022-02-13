using System;
using FrameworkSDK;
using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;

namespace AspNetCore.FrameworkAdapter
{
    public static class AppConfiguratorExtensions
    {
        public static IAspNetCoreAppConfigurator UseAspNetCore([NotNull] this IAppFactory appFactory,
            [NotNull] IHostBuilder hostBuilder)
        {
            if (hostBuilder == null) throw new ArgumentNullException(nameof(hostBuilder));
            appFactory
                .AddComponent<AspNetCoreAppSubSystem>()
                .AddServices(new AspNetCoreServicesModule(hostBuilder));
            return new HostBuilderAppFactory(appFactory, hostBuilder);
        }
    }
    

}