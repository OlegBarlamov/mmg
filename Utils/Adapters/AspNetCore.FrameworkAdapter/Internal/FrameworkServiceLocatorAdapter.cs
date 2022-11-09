using System;
using System.Linq;
using FrameworkSDK.DependencyInjection;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.FrameworkAdapter
{
    internal class FrameworkServiceLocatorAdapter : IServiceLocator
    {
        public IServiceProvider ServiceProvider { get; }
        public IServiceCollection ServiceCollection { get; }

        public FrameworkServiceLocatorAdapter([NotNull] IServiceProvider serviceProvider, [NotNull] IServiceCollection serviceCollection)
        {
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            ServiceCollection = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));
        }
        
        public object CreateInstance(Type type, object[] parameters = null)
        {
            return ActivatorUtilities.CreateInstance(ServiceProvider, type, parameters);
        }

        public object Resolve(Type type, object[] additionalParameters = null)
        {
            if (additionalParameters == null || additionalParameters.Length == 0)
                return ServiceProvider.GetService(type);
            
            var serviceDescriptor = ServiceCollection.Last(descriptor => descriptor.ServiceType == type);
            if (serviceDescriptor.ImplementationInstance != null)
                return serviceDescriptor.ImplementationInstance;

            if (serviceDescriptor.ImplementationFactory != null)
                return serviceDescriptor.ImplementationFactory(ServiceProvider);

            return ActivatorUtilities.CreateInstance(ServiceProvider, serviceDescriptor.ImplementationType,
                additionalParameters);
        }

        public Array ResolveMultiple(Type type)
        {
            return ServiceProvider.GetServices(type).ToArray();
        }

        public bool IsServiceRegistered(Type type)
        {
            return ServiceCollection.Any(descriptor => descriptor.ServiceType == type);
        }
    }
}