using System;
using FrameworkSDK.DependencyInjection;
using FrameworkSDK.DependencyInjection.Default;
using FrameworkSDK.DependencyInjection.Default.Models;
using FrameworkSDK.Logging;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.FrameworkAdapter.Internal
{
    internal class AspNetCoreFrameworkServiceContainer : IFrameworkServiceContainer
    {
        private IServiceCollection AspServiceCollection { get; }
        private IFrameworkServiceContainer FrameworkContainer { get; }

        private IServiceLocator _frameworkServiceLocator;
        private IServiceProvider _aspServiceProvider;

        public AspNetCoreFrameworkServiceContainer([NotNull] IServiceCollection serviceCollection, IFrameworkLogger logger)
        {
            AspServiceCollection = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));
            FrameworkContainer = new DefaultServiceContainer(logger, new RegistrationsDomain());
        }

        public void SetAspServiceProvider(IServiceProvider serviceProvider)
        {
            _aspServiceProvider = serviceProvider;
        }

        public void RegisterInstance(Type serviceType, object instance)
        {
            AspServiceCollection.AddSingleton(serviceType, instance);
            FrameworkContainer.RegisterInstance(serviceType, instance);
        }

        public void RegisterType(Type serviceType, Type implType, ResolveType resolveType = ResolveType.Singletone)
        {
            if (resolveType == ResolveType.Singletone)
                AspServiceCollection.AddSingleton(serviceType, implType);
            else
                AspServiceCollection.AddTransient(serviceType, implType);

            FrameworkContainer.RegisterFactory(serviceType, (locator, type) => _aspServiceProvider.GetService(serviceType), ResolveType.InstancePerResolve);
        }

        public void RegisterFactory(Type serviceType, ServiceFactoryDelegate factory,
            ResolveType resolveType = ResolveType.Singletone)
        {
            if (resolveType == ResolveType.Singletone)
                AspServiceCollection.AddSingleton(serviceType, provider => factory(_frameworkServiceLocator, serviceType));
            else 
                AspServiceCollection.AddTransient(serviceType, provider => factory(_frameworkServiceLocator, serviceType));
            
            FrameworkContainer.RegisterFactory(serviceType, (locator, type) => _aspServiceProvider.GetService(serviceType), ResolveType.InstancePerResolve);
        }

        public void RegisterGeneric(Type genericServiceTypeDefinition, Type genericImplementationTypeDefinition,
            ResolveType resolveType = ResolveType.Singletone)
        {
            FrameworkContainer.RegisterGeneric(genericServiceTypeDefinition, genericImplementationTypeDefinition, resolveType);
            
            //ASP Ioc Doesn't support 
        }

        public void RegisterGenericFactory(Type genericServiceTypeDefinition, ServiceFactoryDelegate factory,
            ResolveType resolveType = ResolveType.Singletone)
        {
            FrameworkContainer.RegisterGenericFactory(genericServiceTypeDefinition, factory, resolveType);
            
            //ASP Ioc Doesn't support
        }

        public void Dispose()
        {
            FrameworkContainer.Dispose();
        }

        public IServiceLocator BuildContainer()
        {
            _frameworkServiceLocator = FrameworkContainer.BuildContainer();
            return _frameworkServiceLocator;
        }

        public IFrameworkServiceContainer CreateScoped(string name = null)
        {
            return FrameworkContainer.CreateScoped(name);
        }
    }
}