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
    public class DefaultAppFactory : ComponentsBasedAppFactory, IAppFactory
    {
        private IFrameworkLogger Logger { get; set; }
        private IFrameworkServiceContainer ServiceContainer { get; set; }
        private ILocalization Localization { get; set; }
        
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

        public override IApp Construct()
        {
            CreateCoreServicesIfNeeded();

            RegisterServices();

            RegisterAppComponents(ServiceContainer, Logger);

            var serviceLocator = BuildServiceLocator();
            var componentsInstances = ConfigureAppComponents(serviceLocator, Logger);
            var subSystemsInstances = ConfigureSubSystems(serviceLocator, Logger);

            Logger.Log("Resolving app runner.", LogCategories.Constructing, FrameworkLogLevel.Info);
            var subSystemsRunner = serviceLocator.Resolve<IAppSubSystemsRunner>();
            return new DefaultAppContainer(subSystemsInstances, subSystemsRunner);
        }

        private IServiceLocator BuildServiceLocator()
        {
            var serviceLocator = ServiceContainer.BuildContainer();
            InitializeAppContext(Logger, serviceLocator);
            return serviceLocator;
        }

        private void RegisterServices()
        {
            Logger.Log("Registering services.", LogCategories.Constructing, FrameworkLogLevel.Info);
            ServiceContainer.RegisterInstance(Logger);
            ServiceContainer.RegisterInstance(Localization);
            ServiceContainer.RegisterInstance(_cmdArgsHolder);
            ServiceContainer.RegisterInstance(ServiceContainer);
            ServiceContainer.RegisterModule<FrameworkCoreServicesModule>();
            RegisterModules(ServiceContainer, Logger);
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
    }
}