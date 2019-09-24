using FrameworkSDK.MonoGame.GameStructure.Controllers;
using FrameworkSDK.MonoGame.GameStructure.Views;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.GameStructure.Mapping
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
