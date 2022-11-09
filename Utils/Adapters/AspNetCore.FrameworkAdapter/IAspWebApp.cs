using FrameworkSDK;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace AspNetCore.FrameworkAdapter
{
    public interface IAspWebApp : IApp, IApplicationBuilder, IEndpointRouteBuilder 
    {
    }
}