using FrameworkSDK.Game.Controllers;
using FrameworkSDK.Game.Views;
using JetBrains.Annotations;

namespace FrameworkSDK.Game.Mapping
{
	public interface IControllersResolver
	{
		[NotNull] IController ResolveByModel([NotNull] object model);

		bool IsModelHasController([NotNull] object model);

		[NotNull] IController ResolveByView([NotNull] IView view);

		bool IsViewHasController([NotNull] IView view);
	}
}
