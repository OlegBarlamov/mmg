using System;
using System.Collections.Generic;
using FrameworkSDK.Constructing;
using FrameworkSDK.Pipelines;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AspNetCore.FrameworkAdapter
{
    public class HostBuilderAppConfiguratorCombination : IAspNetCoreAppConfigurator 
    {
        private IHostBuilder HostBuilder { get; }
        private IAppConfigurator BaseAppConfigurator { get; }

        public HostBuilderAppConfiguratorCombination([NotNull] IHostBuilder hostBuilder, [NotNull] IAppConfigurator baseAppConfigurator)
        {
            HostBuilder = hostBuilder ?? throw new ArgumentNullException(nameof(hostBuilder));
            BaseAppConfigurator = baseAppConfigurator ?? throw new ArgumentNullException(nameof(baseAppConfigurator));
        }

        public void Dispose()
        {
            BaseAppConfigurator.Dispose();
        }

        public Pipeline ConfigurationPipeline => BaseAppConfigurator.ConfigurationPipeline;
        public IAppRunner Configure()
        {
            return BaseAppConfigurator.Configure();
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
            return HostBuilder.Build();
        }

        public IDictionary<object, object> Properties => HostBuilder.Properties;
    }
}