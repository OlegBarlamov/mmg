using System;
using System.Collections.Generic;
using FrameworkSDK;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;

namespace AspNetCore.FrameworkAdapter.Internal
{
    internal class AspWebApp : IAspWebApp
    {
        private IHost NetCore { get; }
        private IApp App { get; }

        public AspWebApp([NotNull] IHost netCore, [NotNull] IApp app)
        {
            NetCore = netCore ?? throw new ArgumentNullException(nameof(netCore));
            App = app ?? throw new ArgumentNullException(nameof(app));
        }
        
        public void Dispose()
        {
            App.Dispose();
            NetCore.Dispose();
        }

        public void Run()
        {
            App.Run();
            NetCore.Run();
        }

        IApplicationBuilder IApplicationBuilder.Use(Func<RequestDelegate, RequestDelegate> middleware)
        {
            return ((IApplicationBuilder)NetCore).Use(middleware);
        }
        
        IApplicationBuilder IApplicationBuilder.New()
        {
            return ((IApplicationBuilder) NetCore).New();
        }
        
        RequestDelegate IApplicationBuilder.Build()
        {
            return ((IApplicationBuilder) NetCore).Build();
        }
        
        IServiceProvider IApplicationBuilder.ApplicationServices
        {
            get => ((IApplicationBuilder)NetCore).ApplicationServices;
            set => ((IApplicationBuilder)NetCore).ApplicationServices = value;
        }
        
        IFeatureCollection IApplicationBuilder.ServerFeatures => ((IApplicationBuilder)NetCore).ServerFeatures;
        
        IDictionary<string, object?> IApplicationBuilder.Properties => ((IApplicationBuilder)NetCore).Properties;
        
        IApplicationBuilder IEndpointRouteBuilder.CreateApplicationBuilder()
        {
            return ((IEndpointRouteBuilder) NetCore).CreateApplicationBuilder();
        }
        
        IServiceProvider IEndpointRouteBuilder.ServiceProvider => ((IEndpointRouteBuilder) NetCore).ServiceProvider;
        
        ICollection<EndpointDataSource> IEndpointRouteBuilder.DataSources => ((IEndpointRouteBuilder) NetCore).DataSources;
    }
}