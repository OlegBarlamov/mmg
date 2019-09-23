using FrameworkSDK.Game.Controllers;
using FrameworkSDK.Game.Views;
using JetBrains.Annotations;

namespace FrameworkSDK.Game.Mapping
{
	public interface IViewsResolver
	{
		[NotNull] IView ResolveByModel([NotNull] object model);

		bool IsModelHasView([NotNull] object model);

		[NotNull] IView ResolveByController([NotNull] IController controller);

		bool IsControllerHasView([NotNull] IController controller);
	}
}
