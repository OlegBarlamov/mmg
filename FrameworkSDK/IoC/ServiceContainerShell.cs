using FrameworkSDK.IoC.Default;
using JetBrains.Annotations;

namespace FrameworkSDK.IoC
{
    internal class ServiceContainerShell : IFrameworkServiceContainer
    {
        [CanBeNull] private IFrameworkServiceContainer CustomServiceContainer { get; set; }

        [NotNull] private IFrameworkServiceContainer ActiveServiceContainer => CustomServiceContainer ?? _defaultServiceContainer;

        [NotNull] private readonly IFrameworkServiceContainer _defaultServiceContainer = new DefaultServiceContainer();

        public void RegisterInstance<T>(T instance)
        {
            ActiveServiceContainer.RegisterInstance(instance);
        }

	    public void RegisterType<TService, TImpl>(ResolveType resolveType = ResolveType.Singletone)
	    {
			ActiveServiceContainer.RegisterType<TService, TImpl>(resolveType);
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
