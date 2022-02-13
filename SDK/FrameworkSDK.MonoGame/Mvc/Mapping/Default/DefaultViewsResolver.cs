using System;
using FrameworkSDK.DependencyInjection;
using FrameworkSDK.MonoGame.Localization;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Mvc
{
	[UsedImplicitly]
    internal class DefaultViewsResolver : IViewsResolver, IDisposable
    {
	    private bool _disposed;

	    private readonly MappingResolver<IView> _internalResolver;

		public DefaultViewsResolver([NotNull] IViewsProvider viewsProvider, [NotNull] IFrameworkServiceContainer serviceContainer)
	    {
		    if (serviceContainer == null) throw new ArgumentNullException(nameof(serviceContainer));
		    if (viewsProvider == null) throw new ArgumentNullException(nameof(viewsProvider));

		    _internalResolver = new MappingResolver<IView>(serviceContainer, nameof(View));

		    var controllersTypes = viewsProvider.GetRegisteredViews();
		    _internalResolver.RegisterTypes(controllersTypes);
		}

		public void Dispose()
		{
			_disposed = true;
			_internalResolver.Dispose();
		}

		public bool IsModelHasView(object model)
	    {
		    if (model == null) throw new ArgumentNullException(nameof(model));
		    if (_disposed) throw new ObjectDisposedException(nameof(DefaultViewsResolver));
		    return _internalResolver.IsModelHasMapping(model);
	    }

	    public IView ResolveByController(IController controller)
	    {
			if (_disposed) throw new ObjectDisposedException(nameof(DefaultViewsResolver));
		    if (controller == null) throw new ArgumentNullException(nameof(controller));

		    try
		    {
			    return _internalResolver.ResolveByController(controller);
		    }
		    catch (Exception e)
		    {
			    throw new MappingException(Strings.Exceptions.Mapping.ViewCreationError, e);
		    }
		}

	    public bool IsControllerHasView(IController controller)
	    {
			if (controller == null) throw new ArgumentNullException(nameof(controller));
		    if (_disposed) throw new ObjectDisposedException(nameof(DefaultViewsResolver));
		    return _internalResolver.IsControllerHasMapping(controller);
		}

	    public IView ResolveByModel(object model)
	    {
		    if (_disposed) throw new ObjectDisposedException(nameof(DefaultViewsResolver));
		    if (model == null) throw new ArgumentNullException(nameof(model));

		    try
		    {
			    return _internalResolver.ResolveByModel(model);
		    }
		    catch (Exception e)
		    {
			    throw new MappingException(Strings.Exceptions.Mapping.ViewCreationError, e);
		    }
	    }
    }
}
