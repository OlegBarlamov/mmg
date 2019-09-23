using FrameworkSDK.Game.Controllers;
using FrameworkSDK.Game.Views;
using JetBrains.Annotations;

namespace FrameworkSDK.Game.Mapping
{
	public interface IModelsResolver
	{
		[NotNull] object ResolveByView([NotNull] IView view);

		bool IsViewHasModel([NotNull] IView view);

		[NotNull] object ResolveByController([NotNull] IController controller);

		bool IsControllerHasModel([NotNull] IController controller);
	}
}
