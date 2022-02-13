using System;
using FrameworkSDK.DependencyInjection;
using FrameworkSDK.MonoGame.Localization;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Mvc
{
	[UsedImplicitly]
	internal class DefaultModelsResolver : IModelsResolver, IDisposable
	{
		private bool _disposed;

		private readonly MappingResolver<object> _internalResolver;

		public DefaultModelsResolver([NotNull] IModelsProvider modelsProvider, [NotNull] IFrameworkServiceContainer serviceContainer)
		{
			if (modelsProvider == null) throw new ArgumentNullException(nameof(modelsProvider));
			if (serviceContainer == null) throw new ArgumentNullException(nameof(serviceContainer));

			_internalResolver = new MappingResolver<object>(serviceContainer, string.Empty);
			var registeredModels = modelsProvider.GetRegisteredModels();
			_internalResolver.RegisterTypes(registeredModels);
		}

		public void Dispose()
		{
			_disposed = true;
			_internalResolver.Dispose();
		}

		public object ResolveByView(IView view)
		{
			if (_disposed) throw new ObjectDisposedException(nameof(DefaultViewsResolver));
			if (view == null) throw new ArgumentNullException(nameof(view));

			try
			{
				return _internalResolver.ResolveByView(view);
			}
			catch (Exception e)
			{
				throw new MappingException(Strings.Exceptions.Mapping.ModelCreationError, e);
			}
		}

		public bool IsViewHasModel(IView view)
		{
			if (_disposed) throw new ObjectDisposedException(nameof(DefaultViewsResolver));
			if (view == null) throw new ArgumentNullException(nameof(view));
			return _internalResolver.IsViewHasMapping(view);
		}

		public object ResolveByController(IController controller)
		{
			if (_disposed) throw new ObjectDisposedException(nameof(DefaultViewsResolver));
			if (controller == null) throw new ArgumentNullException(nameof(controller));

			try
			{
				return _internalResolver.ResolveByController(controller);
			}
			catch (Exception e)
			{
				throw new MappingException(Strings.Exceptions.Mapping.ModelCreationError, e);
			}
		}

		public bool IsControllerHasModel(IController controller)
		{
			if (_disposed) throw new ObjectDisposedException(nameof(DefaultViewsResolver));
			if (controller == null) throw new ArgumentNullException(nameof(controller));
			return _internalResolver.IsControllerHasMapping(controller);
		}
	}
}
