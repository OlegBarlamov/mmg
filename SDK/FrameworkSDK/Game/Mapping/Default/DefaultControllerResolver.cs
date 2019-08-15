using System;
using System.Collections.Generic;
using System.Linq;
using FrameworkSDK.Game.Controllers;
using FrameworkSDK.IoC;
using FrameworkSDK.IoC.Default;
using FrameworkSDK.Localization;
using FrameworkSDK.Services;
using JetBrains.Annotations;
using NetExtensions;

namespace FrameworkSDK.Game.Mapping.Default
{
	[UsedImplicitly]
    internal class DefaultControllerResolver : IControllerResolver, IDisposable
    {
	    private IServiceLocator DefaultServiceLocator { get; }

		private readonly Dictionary<int, Type> _controllersDictionary = new Dictionary<int, Type>();

	    private IServiceLocator ControllersLocator =>
		    _controllersLocator ?? (_controllersLocator = _controllersContainer.BuildContainer());

	    private readonly IFrameworkServiceContainer _controllersContainer;
        private readonly ConstructorFinder _constructorFinder;

	    private IServiceLocator _controllersLocator;
	    private bool _disposed;

		public DefaultControllerResolver([NotNull] IServiceContainerFactory serviceContainerFactory,
		    [NotNull] IAppDomainService appDomainService, [NotNull] IServiceLocator serviceLocator)
	    {
			if (serviceContainerFactory == null) throw new ArgumentNullException(nameof(serviceContainerFactory));
		    if (appDomainService == null) throw new ArgumentNullException(nameof(appDomainService));
		    DefaultServiceLocator = serviceLocator ?? throw new ArgumentNullException(nameof(serviceLocator));

	        _constructorFinder = new ConstructorFinder(DefaultServiceLocator);
            _controllersContainer = serviceContainerFactory.CreateContainer();

		    var registeredTypes = new List<Type>(ExtractAvailableControllerTypes(appDomainService.GetAllTypes()));
		    RegisterControllers(registeredTypes);
		}

	    public void Dispose()
	    {
		    _disposed = true;
		    _controllersDictionary.Clear();
		    _controllersLocator?.Dispose();
	    }

	    IController IControllerResolver.ResolveByModel(object model)
	    {
		    if (_disposed) throw new ObjectDisposedException(nameof(DefaultControllerResolver));
		    if (model == null) throw new ArgumentNullException(nameof(model));

		    try
		    {
			    var hash = GetModelHash(model, out var modelName);
			    var type = ResolveControllerTypeByModelHash(hash, modelName);

			    return (IController)ResolveWithParameter(type, model);
		    }
		    catch (Exception e)
		    {
			    throw new MappingException(Strings.Exceptions.Mapping.ControllerCreationError, e);
		    }
	    }

	    bool IControllerResolver.IsModelHasController(object model)
	    {
		    if (_disposed) throw new ObjectDisposedException(nameof(DefaultControllerResolver));
		    var hash = GetModelHash(model, out _);
		    return _controllersDictionary.ContainsKey(hash);
	    }

		private void RegisterControllers(IReadOnlyList<Type> registeredTypes)
	    {
		    var controllerTitle = nameof(Controller);
		    var controllers = registeredTypes.Where(type => typeof(IController).IsAssignableFrom(type) &&
		                                                     type.Name.EndsWith(controllerTitle, StringComparison.InvariantCultureIgnoreCase));

		    foreach (var controller in controllers)
		    {
			    var modelName = controller.Name.TrimEnd(controllerTitle);
			    var modelHash = modelName.GetHashCode();
			    _controllersContainer.RegisterType(controller, controller, ResolveType.InstancePerResolve);
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

		private static IEnumerable<Type> ExtractAvailableControllerTypes(IEnumerable<Type> domainTypes)
	    {
		    return domainTypes.Where(type =>
			    type.IsClass &&
			    !type.IsAbstract &&
			    !type.IsInterface &&
			    type.IsPublic &&
			    type.IsSubClassOf(typeof(Controller)));
	    }

	    private object ResolveWithParameter(Type type, [NotNull] object parameter)
	    {
		    if (parameter == null) throw new ArgumentNullException(nameof(parameter));

		    var constructor = _constructorFinder.GetConstructorWithParameters(type, parameter.GetType());

		    return ControllersLocator.Resolve(type);
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
