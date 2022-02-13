using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.FrameworkAdapter
{
    [UsedImplicitly]
    internal class FrameworkBasedServiceScopeFactory : IServiceScopeFactory
    {
        [NotNull] public FrameworkBasedContainerBuilder FrameworkBasedContainerBuilder { get; }
        
        public FrameworkBasedServiceScopeFactory([NotNull] FrameworkBasedContainerBuilder frameworkBasedContainerBuilder)
        {
            FrameworkBasedContainerBuilder = frameworkBasedContainerBuilder ?? throw new ArgumentNullException(nameof(frameworkBasedContainerBuilder));
        }
        
        public IServiceScope CreateScope()
        {
            var serviceProvider = (FrameworkBasedServiceProvider)FrameworkBasedContainerBuilder.Create();
            return new ServicesScope(serviceProvider, serviceProvider);
        }

        private class ServicesScope : IServiceScope
        {
            public IDisposable Container { get; }
            public IServiceProvider ServiceProvider { get; }

            public ServicesScope([NotNull] IDisposable container, [NotNull] IServiceProvider serviceProvider)
            {
                Container = container ?? throw new ArgumentNullException(nameof(container));
                ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            }

            public void Dispose()
            {
                Container.Dispose();
            }
        }
    }
}