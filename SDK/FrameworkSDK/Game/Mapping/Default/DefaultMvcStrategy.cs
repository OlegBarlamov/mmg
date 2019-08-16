using System;
using FrameworkSDK.Game.Controllers;
using FrameworkSDK.Game.Views;
using FrameworkSDK.Logging;
using JetBrains.Annotations;

namespace FrameworkSDK.Game.Mapping.Default
{
    [UsedImplicitly]
    internal class DefaultMvcStrategy : IMvcStrategyService
    {
        private IControllersResolver ControllersResolver { get; }
        private IViewsResolver ViewsResolver { get; }

        private readonly ModuleLogger _logger;

        public DefaultMvcStrategy([NotNull] IControllersResolver controllersResolver, [NotNull] IViewsResolver viewsResolver,
            [NotNull] IFrameworkLogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            ControllersResolver = controllersResolver ?? throw new ArgumentNullException(nameof(controllersResolver));
            ViewsResolver = viewsResolver ?? throw new ArgumentNullException(nameof(viewsResolver));

            _logger = new ModuleLogger(_logger, FrameworkLogModule.Mvc);
        }

        public MvcScheme ResolveByModel(object model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            throw new NotImplementedException();
        }

        public MvcScheme ResolveByController(IController controller)
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));

            _logger.Debug("Resolving mvc by controller");

            var scheme = new MvcScheme
            {
                Controller = controller
            };

            if (controller.Model != null)
            {

            }
        }

        public MvcScheme ResolveByView(IView view)
        {
            if (view == null) throw new ArgumentNullException(nameof(view));

            throw new NotImplementedException();
        }
    }
}
