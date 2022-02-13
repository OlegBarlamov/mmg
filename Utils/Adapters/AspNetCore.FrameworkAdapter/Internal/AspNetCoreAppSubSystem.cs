using System;
using FrameworkSDK;
using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;

namespace AspNetCore.FrameworkAdapter
{
    [UsedImplicitly]
    internal class AspNetCoreAppSubSystem : IAppSubSystem
    {
        public IHostBuilder HostBuilder { get; }
        public FrameworkBasedServiceProviderFactory FrameworkBasedServiceProviderFactory { get; }

        public AspNetCoreAppSubSystem([NotNull] IHostBuilder hostBuilder, [NotNull] FrameworkBasedServiceProviderFactory serviceProviderFactory)
        {
            HostBuilder = hostBuilder ?? throw new ArgumentNullException(nameof(hostBuilder));
            FrameworkBasedServiceProviderFactory = serviceProviderFactory ?? throw new ArgumentNullException(nameof(serviceProviderFactory));
        }
        
        public void Dispose()
        {
        }

        public void Configure()
        {
            HostBuilder.UseServiceProviderFactory(FrameworkBasedServiceProviderFactory);
        }

        public void Run()
        {
            HostBuilder.Build().Run();
        }
    }
}