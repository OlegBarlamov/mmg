using System;
using FrameworkSDK.Localization;
using FrameworkSDK.Logging;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Mvc
{
	internal static class ResolversExtensions
	{
		public static IController TryResolveController([NotNull] this IControllersResolver resolver, [NotNull] object model,
			[NotNull] ModuleLogger logger)
		{
			if (resolver == null) throw new ArgumentNullException(nameof(resolver));
			if (model == null) throw new ArgumentNullException(nameof(model));
			if (logger == null) throw new ArgumentNullException(nameof(logger));

			try
			{
				return resolver.ResolveByModel(model);
			}
			catch (Exception e)
			{
				logger.Error(Strings.Errors.Mapping.ResolvingControllerFailed, e, model);
			}
			return null;
		}

		public static IController TryResolveController([NotNull] this IControllersResolver resolver, [NotNull] IView view,
			[NotNull] ModuleLogger logger)
		{
			if (resolver == null) throw new ArgumentNullException(nameof(resolver));
			if (view == null) throw new ArgumentNullException(nameof(view));
			if (logger == null) throw new ArgumentNullException(nameof(logger));

			try
			{
				return resolver.ResolveByView(view);
			}
			catch (Exception e)
			{
				logger.Error(Strings.Errors.Mapping.ResolvingControllerFailed, e, view);
			}
			return null;
		}

		public static IView TryResolveView([NotNull] this IViewsResolver resolver, [NotNull] object model,
			[NotNull] ModuleLogger logger)
		{
			if (resolver == null) throw new ArgumentNullException(nameof(resolver));
			if (model == null) throw new ArgumentNullException(nameof(model));
			if (logger == null) throw new ArgumentNullException(nameof(logger));

			try
			{
				return resolver.ResolveByModel(model);
			}
			catch (Exception e)
			{
				logger.Error(Strings.Errors.Mapping.ResolvingViewFailed, e, model);
			}
			return null;
		}

		public static IView TryResolveView([NotNull] this IViewsResolver resolver, [NotNull] IController controller,
			[NotNull] ModuleLogger logger)
		{
			if (resolver == null) throw new ArgumentNullException(nameof(resolver));
			if (controller == null) throw new ArgumentNullException(nameof(controller));
			if (logger == null) throw new ArgumentNullException(nameof(logger));

			try
			{
				return resolver.ResolveByController(controller);
			}
			catch (Exception e)
			{
				logger.Error(Strings.Errors.Mapping.ResolvingViewFailed, e, controller);
			}
			return null;
		}

		public static object TryResolveModel([NotNull] this IModelsResolver resolver, [NotNull] IView view,
			[NotNull] ModuleLogger logger)
		{
			if (resolver == null) throw new ArgumentNullException(nameof(resolver));
			if (view == null) throw new ArgumentNullException(nameof(view));
			if (logger == null) throw new ArgumentNullException(nameof(logger));

			try
			{
				return resolver.ResolveByView(view);
			}
			catch (Exception e)
			{
				logger.Error(Strings.Errors.Mapping.ResolvingModelFailed, e, view);
			}
			return null;
		}

		public static object TryResolveModel([NotNull] this IModelsResolver resolver, [NotNull] IController controller,
			[NotNull] ModuleLogger logger)
		{
			if (resolver == null) throw new ArgumentNullException(nameof(resolver));
			if (controller == null) throw new ArgumentNullException(nameof(controller));
			if (logger == null) throw new ArgumentNullException(nameof(logger));

			try
			{
				return resolver.ResolveByController(controller);
			}
			catch (Exception e)
			{
				logger.Error(Strings.Errors.Mapping.ResolvingModelFailed, e, controller);
			}
			return null;
		}
	}
}
