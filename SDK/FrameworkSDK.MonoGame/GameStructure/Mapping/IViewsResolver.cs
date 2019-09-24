using FrameworkSDK.MonoGame.GameStructure.Controllers;
using FrameworkSDK.MonoGame.GameStructure.Views;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.GameStructure.Mapping
{
	public interface IViewsResolver
	{
		[NotNull] IView ResolveByModel([NotNull] object model);

		bool IsModelHasView([NotNull] object model);

		[NotNull] IView ResolveByController([NotNull] IController controller);

		bool IsControllerHasView([NotNull] IController controller);
	}
}
