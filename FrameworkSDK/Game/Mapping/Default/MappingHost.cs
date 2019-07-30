using System;
using System.Collections.Generic;
using System.Linq;
using FrameworkSDK.Game.Controllers;
using FrameworkSDK.Game.Views;
using FrameworkSDK.IoC;
using FrameworkSDK.IoC.Default;
using FrameworkSDK.Localization;
using JetBrains.Annotations;
using NetExtensions;

namespace FrameworkSDK.Game.Mapping.Default
{
    internal class MappingHost : IControllerResolver, IViewResolver, IDisposable
    {
        [NotNull] private IFrameworkServiceContainer ServiceContainer { get; }

        private readonly Dictionary<int, Type> _controllersDictionary = new Dictionary<int, Type>();
        private readonly Dictionary<int, Type> _viewsDictionary = new Dictionary<int, Type>();

        private readonly IReadOnlyCollection<Type> _registeredTypes;

        private ConstructorFinder ConstructorFinder =>
            //Резолвим зависимости используя общий serviceLocator
            _constructorFinder ?? (_constructorFinder = new ConstructorFinder(AppSingletone.ServiceLocator));
        private IServiceLocator ServiceLocator =>
            _serviceLocator ?? (_serviceLocator = ServiceContainer.BuildContainer());

        private IServiceLocator _serviceLocator;
        private ConstructorFinder _constructorFinder;
        private bool _disposed;


        public MappingHost([NotNull] IFrameworkServiceContainer serviceContainer,
            [NotNull] IEnumerable<Type> domainTypes)
        {
            if (domainTypes == null) throw new ArgumentNullException(nameof(domainTypes));
            ServiceContainer = serviceContainer ?? throw new ArgumentNullException(nameof(serviceContainer));

            _registeredTypes = new List<Type>(ExtractAvailableViewAndControllerTypes(domainTypes));
        }

        public void Dispose()
        {
            _disposed = true;
            _controllersDictionary.Clear();
            _viewsDictionary.Clear();
            _serviceLocator?.Dispose();
        }

        private static IEnumerable<Type> ExtractAvailableViewAndControllerTypes(IEnumerable<Type> domainTypes)
        {
            return domainTypes.Where(type =>
                type.IsClass &&
                !type.IsAbstract &&
                !type.IsInterface &&
                type.IsPublic &&
                type.IsSubClassOf(typeof(Controller), typeof(View)));
        }

        private void RegisterViews()
        {
            var viewTitle = nameof(View);
            var views = _registeredTypes.Where(type => typeof(IView).IsAssignableFrom(type) &&
                                                       type.Name.EndsWith(viewTitle, StringComparison.InvariantCultureIgnoreCase));

            foreach (var view in views)
            {
                var modelName = view.Name.TrimEnd(viewTitle);
                var modelHash = modelName.GetHashCode();
                ServiceContainer.RegisterType(view, view, ResolveType.InstancePerResolve);
                _viewsDictionary.Add(modelHash, view);
            }
        }

        private void RegisterControllers()
        {
            var controllerTitle = nameof(Controller);
            var controllers = _registeredTypes.Where(type => typeof(IController).IsAssignableFrom(type) &&
                                                             type.Name.EndsWith(controllerTitle, StringComparison.InvariantCultureIgnoreCase));

            foreach (var controller in controllers)
            {
                var modelName = controller.Name.TrimEnd(controllerTitle);
                var modelHash = modelName.GetHashCode();
                ServiceContainer.RegisterType(controller, controller, ResolveType.InstancePerResolve);
                _controllersDictionary.Add(modelHash, controller);
            }
        }

        private Type ResolveControllerTypeByModelHash(int modelHash, string modelTypeName)
        {
            if (!_controllersDictionary.ContainsKey(modelHash))
                throw new MappingException(string.Format(Strings.Exceptions.Mapping.ControllerForModelNotFound,
                    modelTypeName));

            return _controllersDictionary[modelHash];
        }

        private Type ResolveViewTypeByModelHash(int modelHash, string modelTypeName)
        {
            if (!_viewsDictionary.ContainsKey(modelHash))
                throw new MappingException(
                    string.Format(Strings.Exceptions.Mapping.ViewForModelNotFound, modelTypeName));

            return _viewsDictionary[modelHash];
        }

        IController IControllerResolver.ResolveByModel(object model)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(MappingHost));
            if (model == null) throw new ArgumentNullException(nameof(model));

            try
            {
                var hash = GetModelHash(model, out var modelName);
                var type = ResolveControllerTypeByModelHash(hash, modelName);

               return (IController) ResolveWithParameter(type, model);
            }
            catch (Exception e)
            {
                throw new MappingException(Strings.Exceptions.Mapping.ControllerCreationError, e);
            }
        }

        bool IViewResolver.IsModelHasView(object model)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(MappingHost));
            var hash = GetModelHash(model, out _);
            return _viewsDictionary.ContainsKey(hash);
        }

        void IViewResolver.Initialize()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(MappingHost));
            RegisterViews();
        }

        bool IControllerResolver.IsModelHasController(object model)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(MappingHost));
            var hash = GetModelHash(model, out _);
            return _controllersDictionary.ContainsKey(hash);
        }

        void IControllerResolver.Initialize()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(MappingHost));
            RegisterControllers();
        }

        IView IViewResolver.ResolveByModel(object model)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(MappingHost));
            if (model == null) throw new ArgumentNullException(nameof(model));

            try
            {
                var hash = GetModelHash(model, out var modelName);
                var type = ResolveViewTypeByModelHash(hash, modelName);

                return (IView) ResolveWithParameter(type, model);
            }
            catch (Exception e)
            {
                throw new MappingException(Strings.Exceptions.Mapping.ViewCreationError, e);
            }
        }

        private object ResolveWithParameter(Type type, [NotNull] object parameter)
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));

            var constructor = ConstructorFinder.FindConstructorWithParameter(type, parameter.GetType());
            if (constructor != null)
                return ServiceLocator.ResolveWithParameter(type, parameter);

            return ServiceLocator.Resolve(type);
        }

        private int GetModelHash([NotNull] object model, out string modelName)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var type = model.GetType();
            modelName = type.Name;
            var modelTitle = "Model";
            if (modelName.EndsWith(modelTitle, StringComparison.InvariantCultureIgnoreCase))
                modelName = modelName.TrimEnd(modelTitle);

            return modelName.GetHashCode();
        }
    }
}