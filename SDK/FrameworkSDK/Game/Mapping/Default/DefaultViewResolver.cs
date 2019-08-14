using System;
using System.Collections.Generic;
using System.Linq;
using FrameworkSDK.Game.Views;
using FrameworkSDK.IoC;
using FrameworkSDK.IoC.Default;
using FrameworkSDK.Localization;
using FrameworkSDK.Services;
using JetBrains.Annotations;
using NetExtensions;

namespace FrameworkSDK.Game.Mapping.Default
{
	[UsedImplicitly]
    internal class DefaultViewResolver : IViewResolver, IDisposable
    {
	    private IServiceLocator DefaultServiceLocator { get; }

	    private readonly Dictionary<int, Type> _viewsDictionary = new Dictionary<int, Type>();

	    private ConstructorFinder ConstructorFinder =>
		    //Резолвим зависимости используя общий serviceLocator
		    _constructorFinder ?? (_constructorFinder = new ConstructorFinder(DefaultServiceLocator));
	    private IServiceLocator ViewsLocator =>
		    _viewsLocator ?? (_viewsLocator = _viewsContainer.BuildContainer());

	    private readonly IFrameworkServiceContainer _viewsContainer;

	    private IServiceLocator _viewsLocator;
	    private ConstructorFinder _constructorFinder;
	    private bool _disposed;

		public DefaultViewResolver([NotNull] IServiceContainerFactory serviceContainerFactory,
		    [NotNull] IAppDomainService appDomainService, [NotNull] IServiceLocator serviceLocator)
	    {
			if (serviceContainerFactory == null) throw new ArgumentNullException(nameof(serviceContainerFactory));
		    if (appDomainService == null) throw new ArgumentNullException(nameof(appDomainService));
		    DefaultServiceLocator = serviceLocator ?? throw new ArgumentNullException(nameof(serviceLocator));

		    _viewsContainer = serviceContainerFactory.CreateContainer();

		    var registeredTypes = new List<Type>(ExtractAvailableViewTypes(appDomainService.GetAllTypes()));
		    RegisterViews(registeredTypes);
	    }

	    public void Dispose()
	    {
			_disposed = true;
		    _viewsDictionary.Clear();
		    _viewsLocator?.Dispose();
		}

	    bool IViewResolver.IsModelHasView(object model)
	    {
		    if (_disposed) throw new ObjectDisposedException(nameof(DefaultViewResolver));
		    var hash = GetModelHash(model, out _);
		    return _viewsDictionary.ContainsKey(hash);
	    }

	    IView IViewResolver.ResolveByModel(object model)
	    {
		    if (_disposed) throw new ObjectDisposedException(nameof(DefaultViewResolver));
		    if (model == null) throw new ArgumentNullException(nameof(model));

		    try
		    {
			    var hash = GetModelHash(model, out var modelName);
			    var type = ResolveViewTypeByModelHash(hash, modelName);

			    return (IView)ResolveWithParameter(type, model);
		    }
		    catch (Exception e)
		    {
			    throw new MappingException(Strings.Exceptions.Mapping.ViewCreationError, e);
		    }
	    }

		private static IEnumerable<Type> ExtractAvailableViewTypes(IEnumerable<Type> domainTypes)
	    {
		    return domainTypes.Where(type =>
			    type.IsClass &&
			    !type.IsAbstract &&
			    !type.IsInterface &&
			    type.IsPublic &&
			    type.IsSubClassOf(typeof(View)));
	    }

		private void RegisterViews(IReadOnlyCollection<Type> registeredTypes)
	    {
		    var viewTitle = nameof(View);
		    var views = registeredTypes.Where(type => typeof(IView).IsAssignableFrom(type) &&
		                                               type.Name.EndsWith(viewTitle, StringComparison.InvariantCultureIgnoreCase));

		    foreach (var view in views)
		    {
			    var modelName = view.Name.TrimEnd(viewTitle);
			    var modelHash = modelName.GetHashCode();
			    _viewsContainer.RegisterType(view, view, ResolveType.InstancePerResolve);
			    _viewsDictionary.Add(modelHash, view);
		    }
	    }

	    private Type ResolveViewTypeByModelHash(int modelHash, string modelTypeName)
	    {
		    if (!_viewsDictionary.ContainsKey(modelHash))
			    throw new MappingException(
				    string.Format(Strings.Exceptions.Mapping.ViewForModelNotFound, modelTypeName));

		    return _viewsDictionary[modelHash];
	    }

	    private object ResolveWithParameter(Type type, [NotNull] object parameter)
	    {
		    if (parameter == null) throw new ArgumentNullException(nameof(parameter));

		    var constructor = ConstructorFinder.FindConstructorWithParameter(type, parameter.GetType());
		    if (constructor != null)
			    return ViewsLocator.ResolveWithParameter(type, parameter);

		    return ViewsLocator.Resolve(type);
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
