using System;
using FrameworkSDK.Localization;
using FrameworkSDK.Logging;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Mvc
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

        public override IMvcSchemeValidateResult ValidateByModel(object model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            return new MvcSchemeValidateResult
            {
                IsModelExist = true,
                IsViewExist = ViewsResolver.IsModelHasView(model),
                IsControllerExist = ControllersResolver.IsModelHasController(model)
            };
        }

        public override IMvcSchemeValidateResult ValidateByController(IController controller)
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));
            return new MvcSchemeValidateResult
            {
                IsModelExist = ModelsResolver.IsControllerHasModel(controller),
                IsViewExist = ViewsResolver.IsControllerHasView(controller),
                IsControllerExist = true
            };
        }

        public override IMvcSchemeValidateResult ValidateByView(IView view)
        {
            if (view == null) throw new ArgumentNullException(nameof(view));
            return new MvcSchemeValidateResult
            {
                IsModelExist = ModelsResolver.IsViewHasModel(view),
                IsViewExist = true,
                IsControllerExist = ControllersResolver.IsViewHasController(view)
            };
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

			_moduleLogger.Debug(Strings.Info.Mapping.ResolvingMvcByControllerFinished, controller, scheme);

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

		    _moduleLogger.Debug(Strings.Info.Mapping.ResolvingMvcByModelFinished, model, scheme);

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

		    _moduleLogger.Debug(Strings.Info.Mapping.ResolvingMvcByViewFinished, view, scheme);

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
			    _moduleLogger.Debug(Strings.Info.Mapping.ControllerExists, scheme);
				return;
		    }

		    var view = scheme.View;
			var model = scheme.Model;

		    if (model != null && ControllersResolver.IsModelHasController(model))
		    {
			    _moduleLogger.Debug(Strings.Info.Mapping.ResolvingControllerByModel, scheme, model);
			    scheme.Controller = ControllersResolver.TryResolveController(model, _moduleLogger);
			    return;
		    }

		    if (view != null && ControllersResolver.IsViewHasController(view))
		    {
			    _moduleLogger.Debug(Strings.Info.Mapping.ResolvingControllerByView, scheme, view);
			    scheme.Controller = ControllersResolver.TryResolveController(view, _moduleLogger);
			}
	    }

	    private void ResolveView(MvcScheme scheme)
	    {
		    if (!scheme.IsValid())
			    throw new ArgumentException($@"{scheme}", nameof(scheme));

		    if (scheme.View != null)
		    {
		        _moduleLogger.Debug(Strings.Info.Mapping.ViewExists, scheme);
                return;
		    }

		    var controller = scheme.Controller;
		    var model = scheme.Model;

		    if (model != null && ViewsResolver.IsModelHasView(model))
		    {
		        _moduleLogger.Debug(Strings.Info.Mapping.ResolvingViewByModel, scheme, model);
                scheme.View = ViewsResolver.TryResolveView(model, _moduleLogger);
			    return;
		    }

		    if (controller != null && ViewsResolver.IsControllerHasView(controller))
		    {
		        _moduleLogger.Debug(Strings.Info.Mapping.ResolvingViewByController, scheme, controller);
                scheme.View = ViewsResolver.TryResolveView(controller, _moduleLogger);
		    }
		}

	    private void ResolveModel(MvcScheme scheme)
	    {
		    if (!scheme.IsValid())
			    throw new ArgumentException($@"{scheme}", nameof(scheme));

		    if (scheme.Model != null)
		    {
		        _moduleLogger.Debug(Strings.Info.Mapping.ModelExists, scheme);
                return;
		    }

		    var controller = scheme.Controller;
		    var view = scheme.View;

		    if (controller != null && ModelsResolver.IsControllerHasModel(controller))
		    {
		        _moduleLogger.Debug(Strings.Info.Mapping.ResolvingModelByController, scheme, controller);
                scheme.Model = ModelsResolver.TryResolveModel(controller, _moduleLogger);
			    return;
		    }

		    if (view != null && ModelsResolver.IsViewHasModel(view))
		    {
			    _moduleLogger.Debug(Strings.Info.Mapping.ResolvingModelByView, scheme, view);
			    scheme.Model = ModelsResolver.TryResolveModel(view, _moduleLogger);
		    }
		}
	}
}
