using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Mvc
{
	public interface IViewsResolver
	{
		[NotNull] IView ResolveByModel([NotNull] object model);

		bool IsModelHasView([NotNull] object model);

		[NotNull] IView ResolveByController([NotNull] IController controller);

		bool IsControllerHasView([NotNull] IController controller);
	}
}
