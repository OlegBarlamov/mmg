using System;
using System.Collections.Generic;
using FrameworkSDK.DependencyInjection;
using FrameworkSDK.Logging;
using JetBrains.Annotations;

namespace FrameworkSDK.Constructing
{
    public abstract class ComponentsBasedAppFactory : FrameworkContextAppFactory, IAppFactory
    {
        protected List<IServicesModule> ServicesModules { get; } = new List<IServicesModule>();
        
        private readonly List<Type> _componentsTypes = new List<Type>();
        private readonly List<Type> _subsystemsTypes = new List<Type>();
        private readonly List<IAppComponent> _componentsToConfigure = new List<IAppComponent>();
        private readonly List<IAppSubSystem> _subSystemsToConfigure = new List<IAppSubSystem>();
        
        public override IAppFactory AddServices([NotNull] IServicesModule module)
        {
            if (module == null) throw new ArgumentNullException(nameof(module));
            ServicesModules.Add(module);
            return this;
        }

        public override IAppFactory AddComponent<TComponent>()
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

        public override IAppFactory AddComponent([NotNull] IAppComponent appComponent)
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
        
        protected IReadOnlyCollection<IAppSubSystem> ConfigureSubSystems(IServiceLocator serviceLocator, IFrameworkLogger logger)
        {
            var subSystemsInstances = new List<IAppSubSystem>();
            logger.Log("Configuring App subsystems.", LogCategories.Constructing, FrameworkLogLevel.Info);
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

        protected IReadOnlyCollection<IAppComponent> ConfigureAppComponents(IServiceLocator serviceLocator, IFrameworkLogger logger)
        {
            var componentsInstances = new List<IAppComponent>();
            logger.Log("Configuring App components.", LogCategories.Constructing, FrameworkLogLevel.Info);
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

        protected void RegisterModules(IServiceRegistrator serviceRegistrator, IFrameworkLogger logger)
        {
            logger.Log("Registering custom services modules.", LogCategories.Constructing, FrameworkLogLevel.Info);
            foreach (var servicesModule in ServicesModules)
            {
                servicesModule.RegisterServices(serviceRegistrator);
            }
        }

        protected void RegisterAppComponents(IServiceRegistrator serviceRegistrator, IFrameworkLogger logger)
        {
            logger.Log("Registering App components.", LogCategories.Constructing, FrameworkLogLevel.Info);
            foreach (var componentsType in _componentsTypes)
            {
                serviceRegistrator.RegisterType(componentsType, componentsType);
            }

            logger.Log("Registering App subsystems.", LogCategories.Constructing, FrameworkLogLevel.Info);
            foreach (var subsystemsType in _subsystemsTypes)
            {
                serviceRegistrator.RegisterType(subsystemsType, subsystemsType);
            }
        }
    }
}