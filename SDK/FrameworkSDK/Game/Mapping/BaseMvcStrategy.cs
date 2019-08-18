using System;
using FrameworkSDK.Game.Controllers;
using FrameworkSDK.Game.Views;
using FrameworkSDK.Logging;
using JetBrains.Annotations;

namespace FrameworkSDK.Game.Mapping
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

        public MvcScheme ResolveByModel(object model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

			_moduleLogger.Debug($"Resolving mvc by model '{model}'");
	        return ResolveByModelInternal(model);
		}

	    public MvcScheme ResolveByController(IController controller)
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));

            _moduleLogger.Debug($"Resolving mvc by controller '{controller}'");
	        return ResolveByControllerInternal(controller);
        }

        public MvcScheme ResolveByView(IView view)
        {
            if (view == null) throw new ArgumentNullException(nameof(view));

			_moduleLogger.Debug($"Resolving mvc by view '{view}'");
	        return ResolveByViewInternal(view);
		}

	    public virtual void Dispose()
        {
            _moduleLogger.Dispose();
        }

	    [NotNull] protected abstract MvcScheme ResolveByControllerInternal([NotNull] IController controller);

	    [NotNull] protected abstract MvcScheme ResolveByModelInternal([NotNull] object model);

	    [NotNull] protected abstract MvcScheme ResolveByViewInternal([NotNull] IView view);
	}
}
