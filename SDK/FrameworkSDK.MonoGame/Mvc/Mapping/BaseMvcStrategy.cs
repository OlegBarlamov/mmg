using System;
using FrameworkSDK.MonoGame.Localization;
using FrameworkSDK.Logging;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Mvc
{
    public abstract class BaseMvcStrategy : IMvcStrategyService, IDisposable
    {
	    [NotNull] protected IMvcMappingResolver Resolver { get; }
	    [NotNull] protected IFrameworkLogger Logger { get; }

        private readonly ModuleLogger _moduleLogger;

        protected BaseMvcStrategy(
	        [NotNull] IMvcMappingResolver resolver,
	        [NotNull] IFrameworkLogger logger)
        {
	        Resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
	        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
	        _moduleLogger = new ModuleLogger(logger, LogCategories.Mvc);
        }

        public IMvcComponentGroup ResolveByModel(object model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

			_moduleLogger.Debug(Strings.Info.Mapping.ResolvingMvcByModel, model);
	        var result = ResolveByModelInternal(model);
	        _moduleLogger.Debug(Strings.Info.Mapping.ResolvingMvcByModelFinished, model, result);
	        
	        Setup(result);
	        return result;
        }

	    public IMvcComponentGroup ResolveByController(IController controller)
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));

            _moduleLogger.Debug(Strings.Info.Mapping.ResolvingMvcByController, controller);
	        var result = ResolveByControllerInternal(controller);
	        _moduleLogger.Debug(Strings.Info.Mapping.ResolvingMvcByControllerFinished, controller, result);

	        Setup(result);
	        return result;
        }

        public IMvcComponentGroup ResolveByView(IView view)
        {
            if (view == null) throw new ArgumentNullException(nameof(view));

			_moduleLogger.Debug(Strings.Info.Mapping.ResolvingMvcByView, view);
			var result = ResolveByViewInternal(view);
			_moduleLogger.Debug(Strings.Info.Mapping.ResolvingMvcByViewFinished, view, result);
			
			Setup(result);
			return result;
		}

        [NotNull] public abstract IMvcSchemeValidateResult ValidateByModel([NotNull] object model);
        [NotNull] public abstract IMvcSchemeValidateResult ValidateByController([NotNull] IController controller);
        [NotNull] public abstract IMvcSchemeValidateResult ValidateByView([NotNull] IView view);

        public virtual void Dispose()
        {
            _moduleLogger.Dispose();
        }

	    [NotNull] protected abstract IMvcComponentGroup ResolveByControllerInternal([NotNull] IController controller);

	    [NotNull] protected abstract IMvcComponentGroup ResolveByModelInternal([NotNull] object model);

	    [NotNull] protected abstract IMvcComponentGroup ResolveByViewInternal([NotNull] IView view);
	    
	    private void Setup(IMvcComponentGroup componentGroup)
	    {
		    var controller = componentGroup.Controller;
		    var model = componentGroup.Model;
		    var view = componentGroup.View;

		    if (controller != null)
		    {
			    if (view != null)
			    {
				    controller.SetView(view);
				    view.SetController(controller);
			    }
			    if (model != null)
				    controller.SetDataModel(model);
		    }
		    if (view != null && model != null)
			    view.SetDataModel(model);
	    }
	}
}
