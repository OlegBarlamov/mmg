using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Mvc
{
    public interface IMvcStrategyService
    {
        [NotNull] IMvcComponentGroup ResolveByModel([NotNull] object model);
        [NotNull] IMvcComponentGroup ResolveByController([NotNull] IController controller);
        [NotNull] IMvcComponentGroup ResolveByView([NotNull] IView view);

        [NotNull] IMvcSchemeValidateResult ValidateByModel([NotNull] object model);
        [NotNull] IMvcSchemeValidateResult ValidateByController([NotNull] IController controller);
        [NotNull] IMvcSchemeValidateResult ValidateByView([NotNull] IView view);
    }
}
