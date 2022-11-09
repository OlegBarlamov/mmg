using System;
using FrameworkSDK;
using Microsoft.AspNetCore.Builder;

namespace AspNetCore.FrameworkAdapter
{
    public static class AppFactoryExtensions
    {
        public static IAspWebApplicationFactory UseFramework(this WebApplicationBuilder webApplicationBuilder)
        {
            return new DefaultAppFactory(Environment.GetCommandLineArgs()).UseAspWebApp(webApplicationBuilder);
        }
        
        public static IAspWebApplicationFactory UseAspWebApp(this DefaultAppFactory appFactory, WebApplicationBuilder webApplicationBuilder)
        {
            return new AspWebApplicationFactory(appFactory, webApplicationBuilder);
        }

        public static IAspWebApp Asp(this IApp app)
        {
            return (IAspWebApp)app;
        } 
    }
}