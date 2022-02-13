using System;
using FrameworkSDK.DependencyInjection;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.FrameworkAdapter
{
    [UsedImplicitly]
    internal class FrameworkBasedServiceProviderFactory : IServiceProviderFactory<FrameworkBasedContainerBuilder>
    {
        private FrameworkBasedContainerBuilder FrameworkBasedContainerBuilder { get; }
        private bool _isBuilderCreated;
        
        public FrameworkBasedServiceProviderFactory([NotNull] FrameworkBasedContainerBuilder frameworkBasedContainerBuilder)
        {
            FrameworkBasedContainerBuilder = frameworkBasedContainerBuilder ?? throw new ArgumentNullException(nameof(frameworkBasedContainerBuilder));
        }
        
        // Should be executed only once
        public FrameworkBasedContainerBuilder CreateBuilder([NotNull] IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (_isBuilderCreated) throw new InvalidOperationException("CreateBuilder function of IServiceProviderFactory<T> should be executed only once");
            
            _isBuilderCreated = true;
            FrameworkBasedContainerBuilder.PopulateServices(services);

            return FrameworkBasedContainerBuilder;
        }

        public IServiceProvider CreateServiceProvider([NotNull] FrameworkBasedContainerBuilder containerBuilder)
        {
            if (containerBuilder == null) throw new ArgumentNullException(nameof(containerBuilder));
            return containerBuilder.Create();
        }
    }
}