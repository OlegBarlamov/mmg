using FrameworkSDK.Game.Controllers;
using FrameworkSDK.Game.Views;
using JetBrains.Annotations;

namespace FrameworkSDK.Game.Mapping
{
    public interface IMvcStrategyService
    {
        [NotNull] MvcScheme ResolveByModel([NotNull] object model);
        [NotNull] MvcScheme ResolveByController([NotNull] IController controller);
        [NotNull] MvcScheme ResolveByView([NotNull] IView view);
    }
}
