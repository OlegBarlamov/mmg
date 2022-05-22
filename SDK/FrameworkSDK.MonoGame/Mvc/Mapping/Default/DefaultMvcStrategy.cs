using System;
using FrameworkSDK.Logging;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Mvc
{
	// Auto resolving controllers-views-models
	
    [UsedImplicitly]
    internal class DefaultMvcStrategy : BaseMvcStrategy
    {
	    public DefaultMvcStrategy(
			[NotNull] IMvcMappingResolver resolver,
			[NotNull] IFrameworkLogger logger)
			: base(resolver, logger)
		{
		}

		public override IMvcSchemeValidateResult ValidateByModel(object model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            return Resolver.ValidateByModel(model);
        }

        public override IMvcSchemeValidateResult ValidateByController(IController controller)
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));
            return Resolver.ValidateByController(controller);
        }

        public override IMvcSchemeValidateResult ValidateByView(IView view)
        {
            if (view == null) throw new ArgumentNullException(nameof(view));
            return Resolver.ValidateByView(view);
        }

        protected override IMvcComponentGroup ResolveByControllerInternal(IController controller)
        {
	        if (controller == null) throw new ArgumentNullException(nameof(controller));
	        return Resolver.ResolveByController(controller);
        }

		protected override IMvcComponentGroup ResolveByModelInternal(object model)
		{
			if (model == null) throw new ArgumentNullException(nameof(model));
			return Resolver.ResolveByModel(model);
		}

		protected override IMvcComponentGroup ResolveByViewInternal(IView view)
		{
			if (view == null) throw new ArgumentNullException(nameof(view));
			return Resolver.ResolveByView(view);
		}
    }
}
