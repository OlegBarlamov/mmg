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

        public void SetupServiceContainer([CanBeNull] IFrameworkServiceContainer serviceContainer)
        {
            CustomServiceContainer = serviceContainer;
        }

        public void RegisterType<TService, TImpl>()
        {
            ActiveServiceContainer.RegisterType<TService, TImpl>();
        }

        public IServiceLocator BuildContainer()
        {
            return ActiveServiceContainer.BuildContainer();
        }
    }
}
