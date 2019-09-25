using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Mvc
{
	public interface IModelsResolver
	{
		[NotNull] object ResolveByView([NotNull] IView view);

		bool IsViewHasModel([NotNull] IView view);

		[NotNull] object ResolveByController([NotNull] IController controller);

		bool IsControllerHasModel([NotNull] IController controller);
	}
}
