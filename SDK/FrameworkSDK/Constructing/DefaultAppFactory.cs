using System;
using System.Collections.Generic;
using FrameworkSDK.Constructing;
using FrameworkSDK.DependencyInjection;
using FrameworkSDK.DependencyInjection.Default;
using FrameworkSDK.DependencyInjection.Default.Models;
using FrameworkSDK.Localization;
using FrameworkSDK.Logging;
using FrameworkSDK.Services;
using FrameworkSDK.Services.Default;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK
{
    public class DefaultAppFactory : IAppFactory
    {
        public IFrameworkLogger Logger { get; private set; }
        public IFrameworkServiceContainer ServiceContainer { get; private set; }
        public ILocalization Localization { get; private set; }
        
        private readonly List<IServicesModule> _servicesModules = new List<IServicesModule>();
        private readonly List<Type> _componentsTypes = new List<Type>();
        private readonly List<Type> _subsystemsTypes = new List<Type>();
        private readonly ICmdArgsHolder _cmdArgsHolder;

        public DefaultAppFactory():this(new string[0]) { }
        
        public DefaultAppFactory(string[] args)
        {
            _cmdArgsHolder = new DefaultCmdArgsHolder(args);
        }
        
        public DefaultAppFactory UseLogger([NotNull] IFrameworkLogger logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            return this;
        }

        public DefaultAppFactory UseServiceContainer([NotNull] IFrameworkServiceContainer serviceContainer)
        {
            ServiceContainer = serviceContainer ?? throw new ArgumentNullException(nameof(serviceContainer));
            return this;
        }

        public DefaultAppFactory UseLocalization([NotNull] ILocalization localization)
        {
            Localization = localization ?? throw new ArgumentNullException(nameof(localization));
            return this;
        }

        public IApp Construct()
        {
            CreateCoreServicesIfNeeded();

            RegisterServices();

            RegisterAppComponents();

            var serviceLocator = BuildServiceLocator();
            ConfigureAppComponents(serviceLocator);
            var subSystemsInstances = ConfigureSubSystems(serviceLocator);

            var subSystemsRunner = serviceLocator.Resolve<IAppSubSystemsRunner>();
            return new DefaultAppContainer(subSystemsInstances, subSystemsRunner);
        }

        private IReadOnlyCollection<IAppSubSystem> ConfigureSubSystems(IServiceLocator serviceLocator)
        {
            var subSystemsInstances = new List<IAppSubSystem>();
            foreach (var subsystemsType in _subsystemsTypes)
            {
                var component = (IAppSubSystem) serviceLocator.Resolve(subsystemsType);
                component.Configure();
                subSystemsInstances.Add(component);
            }

            return subSystemsInstances;
        }

        private void ConfigureAppComponents(IServiceLocator serviceLocator)
        {
            foreach (var componentsType in _componentsTypes)
            {
                var component = (IAppComponent) serviceLocator.Resolve(componentsType);
                component.Configure();
            }
        }

        private IServiceLocator BuildServiceLocator()
        {
            var serviceLocator = ServiceContainer.BuildContainer();
            AppContext.Initialize(Logger, serviceLocator);
            return serviceLocator;
        }

        private void RegisterAppComponents()
        {
            foreach (var componentsType in _componentsTypes)
            {
                ServiceContainer.RegisterType(componentsType, componentsType);
            }

            foreach (var subsystemsType in _subsystemsTypes)
            {
                ServiceContainer.RegisterType(subsystemsType, subsystemsType);
            }
        }

        private void RegisterServices()
        {
            ServiceContainer.RegisterInstance(Logger);
            ServiceContainer.RegisterInstance(Localization);
            ServiceContainer.RegisterInstance(_cmdArgsHolder);
            ServiceContainer.RegisterInstance(ServiceContainer);
            ServiceContainer.RegisterModule<FrameworkCoreServicesModule>();
            foreach (var servicesModule in _servicesModules)
            {
                servicesModule.RegisterServices(ServiceContainer);
            }
        }

        private void CreateCoreServicesIfNeeded()
        {
            if (Logger == null)
                Logger = new DefaultConsoleLogger();
            if (ServiceContainer == null)
                ServiceContainer = new DefaultServiceContainer(Logger, new RegistrationsDomain());
            if (Localization == null)
                Localization = new DefaultLocalization();
        }

        public IAppFactory AddServices([NotNull] IServicesModule module)
        {
            if (module == null) throw new ArgumentNullException(nameof(module));
            _servicesModules.Add(module);
            return this;
        }

        public IAppFactory AddComponent<TComponent>() where TComponent : class, IAppComponent
        {
            var targetType = typeof(TComponent);
            if (typeof(IAppSubSystem).IsAssignableFrom(targetType))
            {
                _subsystemsTypes.Add(targetType);
            }
            else
            {
                _componentsTypes.Add(targetType);
            }

            return this;
        }
    }
}