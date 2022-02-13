using FrameworkSDK;
using Microsoft.Extensions.Hosting;

namespace AspNetCore.FrameworkAdapter
{
    public interface IAspNetCoreAppConfigurator : IAppFactory, IHostBuilder
    {
    }
}