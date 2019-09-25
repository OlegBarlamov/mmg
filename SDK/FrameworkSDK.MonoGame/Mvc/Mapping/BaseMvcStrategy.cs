using System;
using FrameworkSDK.Localization;
using FrameworkSDK.Logging;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Mvc
{
    public abstract class MvcStrategy : IMvcStrategyService, IDisposable
    {
        [NotNull] protected IControllersResolver ControllersResolver { get; }
        [NotNull] protected IViewsResolver ViewsResolver { get; }
	    [NotNull] protected IModelsResolver ModelsResolver { get; }
		[NotNull] protected IFrameworkLogger Logger { get; }

        private readonly ModuleLogger _moduleLogger;

        protected MvcStrategy([NotNull] IControllersResolver controllersResolver, [NotNull] IViewsResolver viewsResolver, IModelsResolver modelsResolver,
            [NotNull] IFrameworkLogger logger)
        {
            ControllersResolver = controllersResolver ?? throw new ArgumentNullException(nameof(controllersResolver));
            ViewsResolver = viewsResolver ?? throw new ArgumentNullException(nameof(viewsResolver));
	        ModelsResolver = modelsResolver ?? throw new ArgumentNullException(nameof(modelsResolver));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));

			_moduleLogger = new ModuleLogger(logger, FrameworkLogModule.Mvc);
        }

        public IMvcScheme ResolveByModel(object model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

			_moduleLogger.Debug(Strings.Info.Mapping.ResolvingMvcByModel, model);
	        return ResolveByModelInternal(model);
		}

	    public IMvcScheme ResolveByController(IController controller)
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));

            _moduleLogger.Debug(Strings.Info.Mapping.ResolvingMvcByController, controller);
	        return ResolveByControllerInternal(controller);
        }

        public IMvcScheme ResolveByView(IView view)
        {
            if (view == null) throw new ArgumentNullException(nameof(view));

			_moduleLogger.Debug(Strings.Info.Mapping.ResolvingMvcByView, view);
	        return ResolveByViewInternal(view);
		}

        [NotNull] public abstract IMvcSchemeValidateResult ValidateByModel([NotNull] object model);
        [NotNull] public abstract IMvcSchemeValidateResult ValidateByController([NotNull] IController controller);
        [NotNull] public abstract IMvcSchemeValidateResult ValidateByView([NotNull] IView view);

        public virtual void Dispose()
        {
            _moduleLogger.Dispose();
        }

	    [NotNull] protected abstract MvcScheme ResolveByControllerInternal([NotNull] IController controller);

	    [NotNull] protected abstract MvcScheme ResolveByModelInternal([NotNull] object model);

	    [NotNull] protected abstract MvcScheme ResolveByViewInternal([NotNull] IView view);
	}
}
