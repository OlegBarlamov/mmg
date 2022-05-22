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
        private readonly List<IAppComponent> _componentsToConfigure = new List<IAppComponent>();
        private readonly List<IAppSubSystem> _subSystemsToConfigure = new List<IAppSubSystem>();
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
            var componentsInstances = ConfigureAppComponents(serviceLocator);
            var subSystemsInstances = ConfigureSubSystems(serviceLocator);

            Logger.Log("Resolving app runner.", LogCategories.Constructing, FrameworkLogLevel.Info);
            var subSystemsRunner = serviceLocator.Resolve<IAppSubSystemsRunner>();
            return new DefaultAppContainer(subSystemsInstances, subSystemsRunner);
        }

        private IReadOnlyCollection<IAppSubSystem> ConfigureSubSystems(IServiceLocator serviceLocator)
        {
            var subSystemsInstances = new List<IAppSubSystem>();
            Logger.Log("Configuring App subsystems.", LogCategories.Constructing, FrameworkLogLevel.Info);
            foreach (var subsystemsType in _subsystemsTypes)
            {
                var component = (IAppSubSystem) serviceLocator.Resolve(subsystemsType);
                component.Configure();
                subSystemsInstances.Add(component);
            }

            foreach (var subSystem in _subSystemsToConfigure)
            {
                subSystem.Configure();
                subSystemsInstances.Add(subSystem);
            }
            _subSystemsToConfigure.Clear();

            return subSystemsInstances;
        }

        private IReadOnlyCollection<IAppComponent> ConfigureAppComponents(IServiceLocator serviceLocator)
        {
            var componentsInstances = new List<IAppComponent>();
            Logger.Log("Configuring App components.", LogCategories.Constructing, FrameworkLogLevel.Info);
            foreach (var componentsType in _componentsTypes)
            {
                var component = (IAppComponent) serviceLocator.Resolve(componentsType);
                component.Configure();
                componentsInstances.Add(component);
            }

            foreach (var component in _componentsToConfigure)
            {
                component.Configure();
                componentsInstances.Add(component);
            }
            _componentsToConfigure.Clear();
            
            return componentsInstances;
        }

        private IServiceLocator BuildServiceLocator()
        {
            var serviceLocator = ServiceContainer.BuildContainer();
            AppContext.Initialize(Logger, serviceLocator);
            return serviceLocator;
        }

        private void RegisterAppComponents()
        {
            Logger.Log("Registering App components.", LogCategories.Constructing, FrameworkLogLevel.Info);
            foreach (var componentsType in _componentsTypes)
            {
                ServiceContainer.RegisterType(componentsType, componentsType);
            }

            Logger.Log("Registering App subsystems.", LogCategories.Constructing, FrameworkLogLevel.Info);
            foreach (var subsystemsType in _subsystemsTypes)
            {
                ServiceContainer.RegisterType(subsystemsType, subsystemsType);
            }
        }

        private void RegisterServices()
        {
            Logger.Log("Registering services.", LogCategories.Constructing, FrameworkLogLevel.Info);
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
            
            Logger.Log("Core services have been initialized.", LogCategories.Constructing, FrameworkLogLevel.Info);
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

        public IAppFactory AddComponent([NotNull] IAppComponent appComponent)
        {
            if (appComponent == null) throw new ArgumentNullException(nameof(appComponent));
            var targetType = appComponent.GetType();
            if (typeof(IAppSubSystem).IsAssignableFrom(targetType))
            {
                _subSystemsToConfigure.Add((IAppSubSystem)appComponent);
            }
            else
            {
                _componentsToConfigure.Add(appComponent);
            }

            return this;
        }
    }
}