using FrameworkSDK.MonoGame.GameStructure.Controllers;
using FrameworkSDK.MonoGame.GameStructure.Views;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.GameStructure.Mapping
{
	public interface IModelsResolver
	{
		[NotNull] object ResolveByView([NotNull] IView view);

		bool IsViewHasModel([NotNull] IView view);

		[NotNull] object ResolveByController([NotNull] IController controller);

		bool IsControllerHasModel([NotNull] IController controller);
	}
}
