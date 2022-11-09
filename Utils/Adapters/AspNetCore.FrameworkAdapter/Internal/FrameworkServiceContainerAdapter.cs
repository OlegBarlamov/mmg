using System;
using FrameworkSDK.DependencyInjection;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.FrameworkAdapter
{
    internal class FrameworkServiceContainerAdapter : IFrameworkServiceContainer
    {
        public IServiceCollection ServiceCollection { get; }

        private IServiceProvider _serviceProvider;

        public FrameworkServiceContainerAdapter([NotNull] IServiceCollection serviceCollection)
        {
            ServiceCollection = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));
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
            _serviceProvider ??= ServiceCollection.BuildServiceProvider();
            return new FrameworkServiceLocatorAdapter(_serviceProvider, ServiceCollection);
        }

        public IFrameworkServiceContainer CreateScoped(string name = null)
        {
            throw new NotImplementedException();
        }
    }
}