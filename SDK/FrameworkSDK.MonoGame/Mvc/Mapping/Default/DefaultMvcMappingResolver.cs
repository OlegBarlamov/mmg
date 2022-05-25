using System;
using System.Collections.Generic;
using FrameworkSDK.DependencyInjection;
using FrameworkSDK.Logging;
using FrameworkSDK.MonoGame.Localization;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Mvc
{
    [UsedImplicitly]
    public class DefaultMvcMappingResolver : IMvcMappingResolver, IDisposable
    {
        private ModuleLogger Logger { get; }
        
        private readonly Dictionary<Type, MvcTypesDeclaration> _mappingByView = new Dictionary<Type, MvcTypesDeclaration>();
        private readonly Dictionary<Type, MvcTypesDeclaration> _mappingByController = new Dictionary<Type, MvcTypesDeclaration>();
        private readonly Dictionary<Type, MvcTypesDeclaration> _mappingByModel = new Dictionary<Type, MvcTypesDeclaration>();

        private readonly IFrameworkServiceContainer _mappingIocContainer;
        private readonly IServiceLocator _componentsResolver;

        public DefaultMvcMappingResolver([NotNull] IMvcMappingProvider mappingProvider, IFrameworkServiceContainer serviceContainer,
            [NotNull] IFrameworkLogger logger)
        {
            if (mappingProvider == null) throw new ArgumentNullException(nameof(mappingProvider));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            Logger = new ModuleLogger(logger, LogCategories.Mvc);
            _mappingIocContainer = serviceContainer.CreateScoped("mvc_ioc");

            _componentsResolver = CreateResolver(mappingProvider.GetMapping());
        }
        
        public void Dispose()
        {
            _mappingIocContainer.Dispose();
            _mappingByController.Clear();
            _mappingByModel.Clear();
            _mappingByView.Clear();
        }

        private IServiceLocator CreateResolver(IReadOnlyList<MvcTypesDeclaration> mapping)
        {
            foreach (var declaration in mapping)
            {
                RegisterTypes(declaration, _mappingIocContainer);
                FillMapping(declaration);
            }

            return _mappingIocContainer.BuildContainer();
        }

        private void FillMapping(MvcTypesDeclaration types)
        {
            if (types.Controller != null && !_mappingByController.ContainsKey(types.Controller))
                _mappingByController.Add(types.Controller, types);
            if (types.View != null && !_mappingByView.ContainsKey(types.View))
                _mappingByView.Add(types.View, types);
            if (types.Model != null && !_mappingByModel.ContainsKey(types.Model))
                _mappingByModel.Add(types.Model, types);
        }

        public IMvcComponentGroup ResolveByModel(object model)
        {
            var targetType = model.GetType();
            if (!_mappingByModel.ContainsKey(targetType))
            {
                Logger.Debug(Strings.Exceptions.Mapping.MappingForInstanceNotFound, model);
                return new MvcComponentGroup
                {
                    Model = model
                };
            }

            var mapping = _mappingByModel[targetType];

            var view = ResolveView(mapping, model);
            var controller = ResolveController(mapping, model);
            return new MvcComponentGroup
            {
                Model = model,
                View = view,
                Controller = controller
            };
        }

        public IMvcComponentGroup ResolveByController(IController controller)
        {
            var targetType = controller.GetType();
            if (!_mappingByModel.ContainsKey(targetType))
            {
                Logger.Debug(Strings.Exceptions.Mapping.MappingForInstanceNotFound, controller);
                return new MvcComponentGroup
                {
                    Controller = controller
                };
            }

            var mapping = _mappingByController[targetType];

            var model = ResolveModel(mapping);
            var view = ResolveView(mapping, model);
            return new MvcComponentGroup
            {
                Model = model,
                View =  view,
                Controller = controller
            };
        }

        public IMvcComponentGroup ResolveByView(IView view)
        {
            var targetType = view.GetType();
            if (!_mappingByModel.ContainsKey(targetType))
            {
                Logger.Debug(Strings.Exceptions.Mapping.MappingForInstanceNotFound, view);
                return new MvcComponentGroup
                {
                    View = view
                };
            }
            

            var mapping = _mappingByView[targetType];

            var model = ResolveModel(mapping);
            var controller = ResolveController(mapping, model);
            return new MvcComponentGroup
            {
                Model = model,
                View =  view,
                Controller = controller
            };
        }

        public IMvcSchemeValidateResult ValidateByModel(object model)
        {
            var targetType = model.GetType();
            if (!_mappingByModel.ContainsKey(targetType))
            {
                return new MvcSchemeValidateResult
                {
                    IsModelExist = true
                };
            }

            var mapping = _mappingByModel[targetType];
            return new MvcSchemeValidateResult
            {
                IsModelExist = true,
                IsControllerExist = mapping.Controller != null,
                IsViewExist = mapping.View != null
            };
        }

        public IMvcSchemeValidateResult ValidateByController(IController controller)
        {
            var targetType = controller.GetType();
            if (!_mappingByController.ContainsKey(targetType))
            {
                return new MvcSchemeValidateResult
                {
                    IsControllerExist = true
                };
            }

            var mapping = _mappingByModel[targetType];
            return new MvcSchemeValidateResult
            {
                IsModelExist = mapping.Model != null,
                IsControllerExist = true,
                IsViewExist = mapping.View != null
            };
        }

        public IMvcSchemeValidateResult ValidateByView(IView view)
        {
            var targetType = view.GetType();
            if (!_mappingByView.ContainsKey(targetType))
            {
                return new MvcSchemeValidateResult
                {
                    IsViewExist = true
                };
            }

            var mapping = _mappingByModel[targetType];
            return new MvcSchemeValidateResult
            {
                IsModelExist = mapping.View != null,
                IsControllerExist = mapping.Controller != null,
                IsViewExist = true
            };
        }
        
        private IView ResolveView(MvcTypesDeclaration mapping, object model) 
        {
            if (mapping.View == null)
                return null;

            try
            {
                return (IView) _componentsResolver.Resolve(mapping.View, new[] {model});
            }
            catch (Exception e)
            {
                throw new MappingException(Strings.Exceptions.Mapping.ViewCreationError, e);
            }
        }

        private IController ResolveController(MvcTypesDeclaration mapping, object model)
        {
            if (mapping.Controller == null)
                return null;

            try
            {
                return (IController) _componentsResolver.Resolve(mapping.Controller, new[] {model});
            }
            catch (Exception e)
            {
                throw new MappingException(Strings.Exceptions.Mapping.ControllerCreationError, e);
            }
        }
        
        private object ResolveModel(MvcTypesDeclaration mapping) 
        {
            if (mapping.Model == null)
                return null;

            try
            {
                return _componentsResolver.Resolve(mapping.Model);
            }
            catch (Exception e)
            {
                throw new MappingException(Strings.Exceptions.Mapping.ModelCreationError, e);
            }
        }
        
        private static void RegisterTypes(MvcTypesDeclaration types, IFrameworkServiceContainer targetContainer)
        {
            if (types.Controller != null)
                targetContainer.RegisterType(types.Controller, types.Controller);
            // We don't register model types. Model should be provided manually as a parameter (ResolveByModel)
            // if (types.Model != null)
            //     targetContainer.RegisterType(types.Model, types.Model);
            if (types.View != null)
                targetContainer.RegisterType(types.View, types.View);
        }
    }
}