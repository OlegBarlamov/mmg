using System;
using FrameworkSDK.Game.Controllers;
using FrameworkSDK.Game.Views;
using FrameworkSDK.Logging;
using JetBrains.Annotations;

namespace FrameworkSDK.Game.Mapping.Default
{
    [UsedImplicitly]
    internal class DefaultMvcStrategy : MvcStrategy
    {
	    private readonly ModuleLogger _moduleLogger;

		public DefaultMvcStrategy([NotNull] IControllersResolver controllersResolver, [NotNull] IViewsResolver viewsResolver,
			IModelsResolver modelsResolver, [NotNull] IFrameworkLogger logger)
			: base(controllersResolver, viewsResolver, modelsResolver, logger)
		{
			_moduleLogger = new ModuleLogger(Logger, FrameworkLogModule.Mvc);
		}

		protected override MvcScheme ResolveByControllerInternal(IController controller)
		{
			var scheme = new MvcScheme
			{
				Controller = controller,
				Model = controller.Model,
				View = controller.View
			};

			ResolveModel(scheme);
			ResolveView(scheme);

			_moduleLogger.Debug($"Resolving by controller {controller} finished with result: {scheme}.");

			Setup(scheme);
			return scheme;
		}

		protected override MvcScheme ResolveByModelInternal(object model)
		{
			var scheme = new MvcScheme
			{
				Model = model
			};

			ResolveController(scheme);
			ResolveView(scheme);

			_moduleLogger.Debug($"Resolving by model {model} finished with result: {scheme}.");

			Setup(scheme);
			return scheme;
		}

		protected override MvcScheme ResolveByViewInternal(IView view)
		{
			var scheme = new MvcScheme
			{
				Controller = view.Controller,
				Model = view.DataModel,
				View = view
			};

			ResolveModel(scheme);
			ResolveController(scheme);

			_moduleLogger.Debug($"Resolving by view {view} finished with result: {scheme}.");

			Setup(scheme);
			return scheme;
		}

	    private void Setup(MvcScheme scheme)
	    {
		    var controller = scheme.Controller;
		    var model = scheme.Model;
		    var view = scheme.View;

		    if (controller != null)
		    {
			    if (view != null)
			    {
				    controller.SetView(view);
					view.SetController(controller);
			    }
			    if (model != null)
				    controller.SetModel(model);
		    }
		    if (view != null && model != null)
				view.SetDataModel(model);
	    }

	    private void ResolveController(MvcScheme scheme)
	    {
			if (!scheme.IsValid())
				throw new ArgumentException($@"{scheme}", nameof(scheme));

			if (scheme.Controller != null)
		    {
			    _moduleLogger.Debug($"Controller already exists in {scheme}.");
				return;
		    }

		    var view = scheme.View;
			var model = scheme.Model;

		    if (model != null && ControllersResolver.IsModelHasController(model))
		    {
			    _moduleLogger.Debug($"Model exists. Resolving controller for {scheme} by model {model}.");
			    scheme.Controller = ControllersResolver.TryResolveController(model, _moduleLogger);
			    return;
		    }

		    if (view != null && ControllersResolver.IsViewHasController(view))
		    {
			    _moduleLogger.Debug($"Resolving controller for {scheme} by view.");
			    scheme.Controller = ControllersResolver.TryResolveController(view, _moduleLogger);
			}
	    }

	    private void ResolveView(MvcScheme scheme)
	    {
		    if (!scheme.IsValid())
			    throw new ArgumentException($@"{scheme}", nameof(scheme));

		    if (scheme.View != null)
		    {
			    _moduleLogger.Debug($"View already exists in {scheme}.");
			    return;
		    }

		    var controller = scheme.Controller;
		    var model = scheme.Model;

		    if (model != null && ViewsResolver.IsModelHasView(model))
		    {
			    _moduleLogger.Debug($"Model exists. Resolving view for {scheme} by model {model}.");
			    scheme.View = ViewsResolver.TryResolveView(model, _moduleLogger);
			    return;
		    }

		    if (controller != null && ViewsResolver.IsControllerHasView(controller))
		    {
			    _moduleLogger.Debug($"Resolving view for {scheme} by controller.");
			    scheme.View = ViewsResolver.TryResolveView(controller, _moduleLogger);
		    }
		}

	    private void ResolveModel(MvcScheme scheme)
	    {
		    if (!scheme.IsValid())
			    throw new ArgumentException($@"{scheme}", nameof(scheme));

		    if (scheme.Model != null)
		    {
			    _moduleLogger.Debug($"Model already exists in {scheme}.");
			    return;
		    }

		    var controller = scheme.Controller;
		    var view = scheme.View;

		    if (controller != null && ModelsResolver.IsControllerHasModel(controller))
		    {
			    _moduleLogger.Debug($"Controller exists. Resolving model for {scheme} by controller {controller}.");
			    scheme.Model = ModelsResolver.TryResolveModel(controller, _moduleLogger);
			    return;
		    }

		    if (view != null && ModelsResolver.IsViewHasModel(view))
		    {
			    _moduleLogger.Debug($"Resolving model for {scheme} by view.");
			    scheme.Model = ModelsResolver.TryResolveModel(view, _moduleLogger);
		    }
		}
	}
}
