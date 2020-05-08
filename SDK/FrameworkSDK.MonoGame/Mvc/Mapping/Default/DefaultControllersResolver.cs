using System;
using FrameworkSDK.IoC;
using FrameworkSDK.MonoGame.Localization;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Mvc
{
	[UsedImplicitly]
    internal class DefaultControllersResolver : IControllersResolver, IDisposable
	{
		private bool _disposed;

		private readonly MappingResolver<IController> _internalResolver;

		public DefaultControllersResolver([NotNull] IControllersProvider controllersProvider, [NotNull] IFrameworkServiceContainer serviceContainer)
	    {
			if (serviceContainer == null) throw new ArgumentNullException(nameof(serviceContainer));
		    if (controllersProvider == null) throw new ArgumentNullException(nameof(controllersProvider));

			_internalResolver = new MappingResolver<IController>(serviceContainer, nameof(Controller));

		    var controllersTypes = controllersProvider.GetRegisteredControllers();
		    _internalResolver.RegisterTypes(controllersTypes);
		}

	    public void Dispose()
	    {
			_disposed = true;
			_internalResolver.Dispose();
		}

	    public IController ResolveByModel(object model)
	    {
		    if (model == null) throw new ArgumentNullException(nameof(model));
		    if (_disposed) throw new ObjectDisposedException(nameof(DefaultControllersResolver));

			try
		    {
			    return _internalResolver.ResolveByModel(model);
		    }
		    catch (Exception e)
		    {
			    throw new MappingException(Strings.Exceptions.Mapping.ControllerCreationError, e);
		    }
	    }

	    public bool IsModelHasController(object model)
	    {
		    if (model == null) throw new ArgumentNullException(nameof(model));
		    if (_disposed) throw new ObjectDisposedException(nameof(DefaultControllersResolver));
			return _internalResolver.IsModelHasMapping(model);
	    }

		public IController ResolveByView(IView view)
		{
			if (view == null) throw new ArgumentNullException(nameof(view));
			if (_disposed) throw new ObjectDisposedException(nameof(DefaultControllersResolver));

			try
			{
				return _internalResolver.ResolveByView(view);
			}
			catch (Exception e)
			{
				throw new MappingException(Strings.Exceptions.Mapping.ControllerCreationError, e);
			}
		}

		public bool IsViewHasController(IView view)
		{
			if (view == null) throw new ArgumentNullException(nameof(view));
			if (_disposed) throw new ObjectDisposedException(nameof(DefaultControllersResolver));
			return _internalResolver.IsViewHasMapping(view);
		}
	}
}
