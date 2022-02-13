using System;
using System.Collections.Generic;
using FrameworkSDK.DependencyInjection;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AspNetCore.FrameworkAdapter
{
    internal class AspNetCoreServicesModule : IServicesModule
    {
        [NotNull] public IHostBuilder HostBuilder { get; }

        public AspNetCoreServicesModule([NotNull] IHostBuilder hostBuilder)
        {
            HostBuilder = hostBuilder ?? throw new ArgumentNullException(nameof(hostBuilder));
        }

        public void RegisterServices(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.RegisterInstance(HostBuilder);
            serviceRegistrator.RegisterType<FrameworkBasedServiceProviderFactory, FrameworkBasedServiceProviderFactory>();
            serviceRegistrator.RegisterType<FrameworkBasedContainerBuilder, FrameworkBasedContainerBuilder>();
            serviceRegistrator.RegisterType<IServiceScopeFactory, FrameworkBasedServiceScopeFactory>();
            serviceRegistrator.RegisterGenericFactory(typeof(IEnumerable<>), ResolveEnumerable);
        }

        private object ResolveEnumerable(IServiceLocator servicelocator, Type requestedtype)
        {
            var itemsType = requestedtype.GetGenericArguments()[0];
            return servicelocator.ResolveMultiple(itemsType);
        }
    }
}