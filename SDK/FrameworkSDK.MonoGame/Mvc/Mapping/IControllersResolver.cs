using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Mvc
{
	public interface IControllersResolver
	{
		[NotNull] IController ResolveByModel([NotNull] object model);

		bool IsModelHasController([NotNull] object model);

		[NotNull] IController ResolveByView([NotNull] IView view);

		bool IsViewHasController([NotNull] IView view);
	}
}
