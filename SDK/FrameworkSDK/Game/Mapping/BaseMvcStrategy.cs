using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        [NotNull] protected IFrameworkLogger Logger { get; }

        private readonly ModuleLogger _moduleLogger;

        protected MvcStrategy([NotNull] IControllersResolver controllersResolver, [NotNull] IViewsResolver viewsResolver,
            [NotNull] IFrameworkLogger logger)
        {
            ControllersResolver = controllersResolver ?? throw new ArgumentNullException(nameof(controllersResolver));
            ViewsResolver = viewsResolver ?? throw new ArgumentNullException(nameof(viewsResolver));
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            _moduleLogger = new ModuleLogger(_moduleLogger, FrameworkLogModule.Mvc);
        }

        public MvcScheme ResolveByModel(object model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            throw new NotImplementedException();
        }

        public MvcScheme ResolveByController(IController controller)
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));

            _moduleLogger.Debug("Resolving mvc by controller");

        }

        public MvcScheme ResolveByView(IView view)
        {
            if (view == null) throw new ArgumentNullException(nameof(view));

            throw new NotImplementedException();
        }

        public virtual void Dispose()
        {
            _moduleLogger.Dispose();
        }
    }
}
