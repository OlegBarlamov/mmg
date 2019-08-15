using System;
using FrameworkSDK.Game.Views;
using FrameworkSDK.IoC;
using FrameworkSDK.Localization;
using JetBrains.Annotations;

namespace FrameworkSDK.Game.Mapping.Default
{
	[UsedImplicitly]
    internal class DefaultViewsResolver : IViewsResolver, IDisposable
    {
	    private bool _disposed;

	    private readonly MappingResolver<IView> _internalResolver;

		public DefaultViewsResolver([NotNull] IViewsProvider viewsProvider, [NotNull] IFrameworkServiceContainer serviceContainer,
			[NotNull] IServiceLocator serviceLocator)
	    {
		    if (serviceContainer == null) throw new ArgumentNullException(nameof(serviceContainer));
		    if (serviceLocator == null) throw new ArgumentNullException(nameof(serviceLocator));
		    if (viewsProvider == null) throw new ArgumentNullException(nameof(viewsProvider));

		    _internalResolver = new MappingResolver<IView>(serviceContainer, nameof(View), "Model");

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
		    if (_disposed) throw new ObjectDisposedException(nameof(DefaultViewsResolver));
		    return _internalResolver.IsModelHasMapping(model);
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
