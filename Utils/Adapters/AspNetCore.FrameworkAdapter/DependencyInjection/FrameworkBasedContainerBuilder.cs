using System;
using System.Collections.Generic;
using FrameworkSDK.DependencyInjection;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.FrameworkAdapter
{
    internal class FrameworkBasedContainerBuilder
    {
        public IFrameworkServiceContainer ServiceContainer { get; }

        private readonly List<ServiceDescriptor> _scopedServices = new List<ServiceDescriptor>();

        public FrameworkBasedContainerBuilder(
            [NotNull] IFrameworkServiceContainer serviceContainer)
        {
            ServiceContainer = serviceContainer ?? throw new ArgumentNullException(nameof(serviceContainer));
        }

        public void PopulateServices(IServiceCollection services)
        {
            foreach (var service in services)
            {
                if (service.Lifetime == ServiceLifetime.Singleton)
                {
                    RegisterService(ServiceContainer, service, ResolveType.Singletone);
                } 
                else if (service.Lifetime == ServiceLifetime.Transient)
                {
                    RegisterService(ServiceContainer, service, ResolveType.InstancePerResolve);
                }
                else
                {
                    _scopedServices.Add(service);
                }
            }
        }
        
        public IServiceProvider Create()
        {
            var scopedContainer = ServiceContainer.CreateScoped();
            RegisterScopedServices(scopedContainer, _scopedServices);
            var serviceProvider = new FrameworkBasedServiceProvider(scopedContainer);
            scopedContainer.RegisterInstance<IServiceProvider>(serviceProvider);
            return serviceProvider.Build();
        }

        private void RegisterScopedServices(IServiceRegistrator serviceRegistrator, IReadOnlyCollection<ServiceDescriptor> scopedServices)
        {
            foreach (var scopedService in scopedServices)
            {
                RegisterService(serviceRegistrator, scopedService, ResolveType.Singletone);
            }
        }

        private void RegisterService(IServiceRegistrator serviceRegistrator, ServiceDescriptor serviceDescriptor,
            ResolveType resolveType)
        {
            if (serviceDescriptor.ServiceType.IsGenericTypeDefinition)
                RegisterServiceGenericDefinition(serviceRegistrator, serviceDescriptor, resolveType);
            else 
                RegisterServiceSimple(serviceRegistrator, serviceDescriptor, resolveType);
        }

        private void RegisterServiceGenericDefinition(IServiceRegistrator serviceRegistrator, ServiceDescriptor serviceDescriptor,
            ResolveType resolveType)
        {
            serviceRegistrator.RegisterGeneric(serviceDescriptor.ServiceType, serviceDescriptor.ImplementationType, resolveType);
        }
        
        private void RegisterServiceSimple(IServiceRegistrator serviceRegistrator, ServiceDescriptor serviceDescriptor, ResolveType resolveType)
        {
            if (serviceDescriptor.ImplementationType != null)
                serviceRegistrator.RegisterType(serviceDescriptor.ServiceType, serviceDescriptor.ImplementationType, resolveType);
            else if (serviceDescriptor.ImplementationInstance != null)
            {
                if (resolveType != ResolveType.Singletone)
                    throw new AspNetCoreDependencyInjectionRegistrationException($"Wrong resolveType: ImplementationInstance can be used only as Singletone in descriptor {serviceDescriptor.ServiceType}");
                
                serviceRegistrator.RegisterInstance(serviceDescriptor.ServiceType, serviceDescriptor.ImplementationInstance);   
            }
            else if (serviceDescriptor.ImplementationFactory != null)
            {
                var targetResolveType = serviceDescriptor.Lifetime == ServiceLifetime.Transient
                    ? ResolveType.InstancePerResolve
                    : ResolveType.Singletone;
                serviceRegistrator.RegisterFactory(serviceDescriptor.ServiceType, (locator, requestedType) => serviceDescriptor.ImplementationFactory(locator.Resolve<IServiceProvider>()), targetResolveType);
            }
            else
                throw new AspNetCoreDependencyInjectionRegistrationException($"Implementation descriptor has not been found in service descriptor {serviceDescriptor.ServiceType}");
        }
    }
}