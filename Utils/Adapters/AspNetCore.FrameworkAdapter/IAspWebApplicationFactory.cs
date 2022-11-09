using FrameworkSDK;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.FrameworkAdapter
{
    public interface IAspWebApplicationFactory : IAppFactory, IServiceCollection
    {
    }
}