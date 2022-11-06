using FrameworkSDK;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace AspNetCore.FrameworkAdapter
{
    public interface IAspNetCoreApp : IApp, IApplicationBuilder, IEndpointRouteBuilder 
    {
    }
}