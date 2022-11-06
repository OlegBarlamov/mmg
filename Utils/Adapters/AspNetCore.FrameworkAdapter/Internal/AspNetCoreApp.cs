using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;

namespace AspNetCore.FrameworkAdapter.Internal
{
    internal class AspNetCoreApp : IAspNetCoreApp
    {
        public WebApplication AspNetApp { get; }

        public AspNetCoreApp([NotNull] WebApplication aspNetApp)
        {
            AspNetApp = aspNetApp ?? throw new ArgumentNullException(nameof(aspNetApp));
        }
        
        public void Dispose()
        {
        }

        public void Run()
        {
            AspNetApp.Run();
        }

        IApplicationBuilder IApplicationBuilder.Use(Func<RequestDelegate, RequestDelegate> middleware)
        {
            return ((IApplicationBuilder)AspNetApp).Use(middleware);
        }

        IApplicationBuilder IApplicationBuilder.New()
        {
            return ((IApplicationBuilder) AspNetApp).New();
        }

        RequestDelegate IApplicationBuilder.Build()
        {
            return ((IApplicationBuilder) AspNetApp).Build();
        }

        IServiceProvider IApplicationBuilder.ApplicationServices
        {
            get => ((IApplicationBuilder)AspNetApp).ApplicationServices;
            set => ((IApplicationBuilder)AspNetApp).ApplicationServices = value;
        }

        IFeatureCollection IApplicationBuilder.ServerFeatures => ((IApplicationBuilder)AspNetApp).ServerFeatures;

        IDictionary<string, object?> IApplicationBuilder.Properties => ((IApplicationBuilder)AspNetApp).Properties;

        IApplicationBuilder IEndpointRouteBuilder.CreateApplicationBuilder()
        {
            return ((IEndpointRouteBuilder) AspNetApp).CreateApplicationBuilder();
        }

        IServiceProvider IEndpointRouteBuilder.ServiceProvider => ((IEndpointRouteBuilder) AspNetApp).ServiceProvider;

        ICollection<EndpointDataSource> IEndpointRouteBuilder.DataSources => ((IEndpointRouteBuilder) AspNetApp).DataSources;
    }
}