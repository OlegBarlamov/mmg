using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Mvc
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
