using FrameworkSDK.Game.Controllers;
using FrameworkSDK.Game.Views;
using JetBrains.Annotations;

namespace FrameworkSDK.Game.Mapping
{
    public interface IMvcStrategyService
    {
        [NotNull] IMvcScheme ResolveByModel([NotNull] object model);
        [NotNull] IMvcScheme ResolveByController([NotNull] IController controller);
        [NotNull] IMvcScheme ResolveByView([NotNull] IView view);

        [NotNull] IMvcSchemeValidateResult ValidateByModel([NotNull] object model);
        [NotNull] IMvcSchemeValidateResult ValidateByController([NotNull] IController controller);
        [NotNull] IMvcSchemeValidateResult ValidateByView([NotNull] IView view);
    }
}
