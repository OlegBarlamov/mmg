using System;
using System.Linq;
using FrameworkSDK.DependencyInjection;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.FrameworkAdapter
{
    internal class FrameworkServiceContainerAdapter : IFrameworkServiceContainer
    {
        public WebApplicationBuilder ApplicationBuilder { get; }
        public IServiceCollection ServiceCollection { get; }
        
        public WebApplication WebApplication { get; private set; }

        private IServiceProvider _serviceProvider;

        public FrameworkServiceContainerAdapter([NotNull] WebApplicationBuilder applicationBuilder)
        {
            ApplicationBuilder = applicationBuilder ?? throw new ArgumentNullException(nameof(applicationBuilder));
            ServiceCollection = ApplicationBuilder.Services;
        }
        
        public void RegisterInstance(Type serviceType, object instance)
        {
            ServiceCollection.AddSingleton(serviceType, instance);
        }

        public void RegisterType(Type serviceType, Type implType, ResolveType resolveType = ResolveType.Singletone)
        {
            if (resolveType == ResolveType.Singletone)
            {
                ServiceCollection.AddSingleton(serviceType, implType);
            }
            else
            {
                ServiceCollection.AddTransient(serviceType, implType);
            }
        }

        public void RegisterFactory(Type serviceType, ServiceFactoryDelegate factory,
            ResolveType resolveType = ResolveType.Singletone)
        {
            if (resolveType == ResolveType.Singletone)
            {
                ServiceCollection.AddSingleton(serviceType, provider =>  factory(new FrameworkServiceLocatorAdapter(provider, ServiceCollection), serviceType));
            }
            else
            {
                ServiceCollection.AddTransient(serviceType, provider =>  factory(new FrameworkServiceLocatorAdapter(provider, ServiceCollection), serviceType));
            }
        }

        public void Dispose()
        {
        }

        public IServiceLocator BuildContainer()
        {
            if (WebApplication == null)
            {
                WebApplication = ApplicationBuilder.Build();
                _serviceProvider = WebApplication.Services;
            }
            return new FrameworkServiceLocatorAdapter(_serviceProvider, ServiceCollection);
        }

        public IFrameworkServiceContainer CreateScoped(string name = null)
        {
            throw new NotImplementedException();
        }

        public bool ContainsRegistrationForType(Type type)
        {
            return ServiceCollection.Any(descriptor => descriptor.ServiceType == type);
        }
    }
}