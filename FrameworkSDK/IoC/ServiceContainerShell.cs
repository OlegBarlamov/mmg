using System;
using FrameworkSDK.IoC.Default;
using JetBrains.Annotations;

namespace FrameworkSDK.IoC
{
    internal class ServiceContainerShell : IFrameworkServiceContainer
    {
        [CanBeNull] private IFrameworkServiceContainer CustomServiceContainer { get; set; }

        [NotNull] private IFrameworkServiceContainer ActiveServiceContainer => CustomServiceContainer ?? _defaultServiceContainer;

        [NotNull] private readonly IFrameworkServiceContainer _defaultServiceContainer = new DefaultServiceContainer();

        public void RegisterInstance(Type serviceType, object instance)
        {
            ActiveServiceContainer.RegisterInstance(serviceType, instance);
        }

        public void RegisterType(Type serviceType, Type implType, ResolveType resolveType = ResolveType.Singletone)
        {
            ActiveServiceContainer.RegisterType(serviceType, implType, resolveType);
        }

        public void SetupServiceContainer([CanBeNull] IFrameworkServiceContainer serviceContainer)
        {
            CustomServiceContainer = serviceContainer;
        }

        public IServiceLocator BuildContainer()
        {
            return ActiveServiceContainer.BuildContainer();
        }

	    public void Dispose()
	    {
		    ActiveServiceContainer.Dispose();
	    }

        
    }
}
