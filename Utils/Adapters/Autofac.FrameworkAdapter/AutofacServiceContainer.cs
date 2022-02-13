using System;
using FrameworkSDK.DependencyInjection;
using JetBrains.Annotations;

namespace Autofac.FrameworkAdapter
{
    public class AutofacServiceContainer : IFrameworkServiceContainer
    {
        public ContainerBuilder ContainerBuilder { get; }
        private AutofacServiceLocator _serviceLocator;

        public AutofacServiceContainer([NotNull] ContainerBuilder containerBuilder, string name = null)
        {
            ContainerBuilder = containerBuilder ?? throw new ArgumentNullException(nameof(containerBuilder));
        }
        
        public void RegisterInstance(Type serviceType, object instance)
        {
            ContainerBuilder.RegisterInstance(instance).As(serviceType);
        }

        public void RegisterType(Type serviceType, Type implType, ResolveType resolveType = ResolveType.Singletone)
        {
            switch (resolveType)
            {
                case ResolveType.Singletone:
                    ContainerBuilder.RegisterType(implType).As(serviceType).SingleInstance();
                    break;
                case ResolveType.InstancePerResolve:
                    ContainerBuilder.RegisterType(implType).As(serviceType).InstancePerDependency();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(resolveType), resolveType, null);
            }
        }

        public void Dispose()
        {
            _serviceLocator?.Dispose();
        }

        public IServiceLocator BuildContainer()
        {
            _serviceLocator = BuildAutofacServiceLocator();
            return _serviceLocator;
        }

        public IFrameworkServiceContainer CreateScoped(string name = null)
        {
            if (_serviceLocator == null)
                _serviceLocator = BuildAutofacServiceLocator();
         
            var childContainerBuilder = new ContainerBuilder();
            foreach (var registration in _serviceLocator.Container.ComponentRegistry.Registrations)
            {
                childContainerBuilder.ComponentRegistryBuilder.Register(registration);
            }
            return new AutofacServiceContainer(childContainerBuilder);
        }

        private AutofacServiceLocator BuildAutofacServiceLocator()
        {
            var container = ContainerBuilder.Build();
            var serviceLocator = new AutofacServiceLocator(container);
            return serviceLocator;
        }
    }
}