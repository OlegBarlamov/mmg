using System;
using AspNetCore.FrameworkAdapter.Internal;
using FrameworkSDK;
using FrameworkSDK.Constructing;
using FrameworkSDK.DependencyInjection;
using FrameworkSDK.Localization;
using FrameworkSDK.Logging;
using FrameworkSDK.Services;
using FrameworkSDK.Services.Default;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspNetCore.FrameworkAdapter
{
    public class AspNetCoreAppFactory : ComponentsBasedAppFactory, IAppFactory
    {
        public IServiceCollection Services => _webApplicationBuilder.Services;
        
        private ILoggerFactory LoggerFactory { get; set; }
        
        private readonly WebApplicationBuilder _webApplicationBuilder;
        private readonly ICmdArgsHolder _cmdArgsHolder;

        private AspNetCoreFrameworkServiceContainer _serviceContainer;
        private ILocalization _localization;
        private IFrameworkLogger _logger;
        
        public AspNetCoreAppFactory():this(new string[0]) { }
        
        public AspNetCoreAppFactory(string[] args)
        {
            _cmdArgsHolder = new DefaultCmdArgsHolder(args);
            
            _webApplicationBuilder = WebApplication.CreateBuilder(args);
        }

        public AspNetCoreAppFactory SetLoggerFactory([NotNull] ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            return this;
        }

        public override IApp Construct()
        {
            CreateCoreServices();
            
            RegisterServices();
            
            RegisterAppComponents(_serviceContainer, _logger);
            
            var aspNetApp = _webApplicationBuilder.Build();
            var aspServiceProvider = aspNetApp.Services;
            _serviceContainer.SetAspServiceProvider(aspServiceProvider);
            
            var serviceLocator = BuildServiceLocator();
            var componentsInstances = ConfigureAppComponents(serviceLocator, _logger);
            var subSystemsInstances = ConfigureSubSystems(serviceLocator, _logger);
            
            return new AspNetCoreApp(aspNetApp);
        }

        private void RegisterServices()
        {
            _logger.Log("Registering services.", LogCategories.Constructing, FrameworkLogLevel.Info);
            _serviceContainer.RegisterInstance(_logger);
            _serviceContainer.RegisterInstance(_localization);
            _serviceContainer.RegisterInstance(_cmdArgsHolder);
            _serviceContainer.RegisterInstance(_serviceContainer);
            _serviceContainer.RegisterModule<FrameworkCoreServicesModule>();
            
            RegisterModules(_serviceContainer, _logger);
        }

        private void CreateCoreServices()
        {
            if (_localization == null)
                _localization = new DefaultLocalization();
            if (LoggerFactory == null)
                LoggerFactory = CreateDefaultLoggerFactory();
            
            _logger = new FrameworkLoggerWrapper(LoggerFactory);
            _serviceContainer = new AspNetCoreFrameworkServiceContainer(Services, _logger);
        }
        
        private IServiceLocator BuildServiceLocator()
        {
            var serviceLocator = _serviceContainer.BuildContainer();
            InitializeAppContext(_logger, serviceLocator);
            return serviceLocator;
        }

        private static ILoggerFactory CreateDefaultLoggerFactory()
        {
            return Microsoft.Extensions.Logging.LoggerFactory.Create(loggingBuilder => loggingBuilder
                .SetMinimumLevel(LogLevel.Trace)
                .AddConsole());
        }
    }
}