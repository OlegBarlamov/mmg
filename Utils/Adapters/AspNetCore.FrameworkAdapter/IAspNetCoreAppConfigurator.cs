using FrameworkSDK.Constructing;
using Microsoft.Extensions.Hosting;

namespace AspNetCore.FrameworkAdapter
{
    public interface IAspNetCoreAppConfigurator : IAppConfigurator, IHostBuilder
    {
    }
}