using System;
using System.Collections.Generic;
using FrameworkSDK;
using FrameworkSDK.Constructing;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AspNetCore.FrameworkAdapter
{
    internal class HostBuilderAppFactory : AppFactoryWrapper, IAspNetCoreAppConfigurator 
    {
        private IHostBuilder HostBuilder { get; }

        public HostBuilderAppFactory([NotNull] IAppFactory appFactory, [NotNull] IHostBuilder hostBuilder)
            :base(appFactory)
        {
            HostBuilder = hostBuilder ?? throw new ArgumentNullException(nameof(hostBuilder));
        }

        public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate)
        {
            return HostBuilder.ConfigureHostConfiguration(configureDelegate);
        }

        public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
        {
            return HostBuilder.ConfigureAppConfiguration(configureDelegate);
        }

        public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
        {
            return HostBuilder.ConfigureServices(configureDelegate);
        }

        public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory)
        {
            return HostBuilder.UseServiceProviderFactory(factory);
        }

        public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory)
        {
            return HostBuilder.UseServiceProviderFactory(factory);
        }

        public IHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate)
        {
            return HostBuilder.ConfigureContainer(configureDelegate);
        }

        public IHost Build()
        {
            throw new WrongHostBuildExecutionException();
        }

        public IDictionary<object, object> Properties => HostBuilder.Properties;
    }
}