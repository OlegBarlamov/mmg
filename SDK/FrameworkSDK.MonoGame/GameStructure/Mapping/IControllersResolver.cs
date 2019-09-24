using FrameworkSDK.MonoGame.GameStructure.Controllers;
using FrameworkSDK.MonoGame.GameStructure.Views;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.GameStructure.Mapping
{
	public interface IControllersResolver
	{
		[NotNull] IController ResolveByModel([NotNull] object model);

		bool IsModelHasController([NotNull] object model);

		[NotNull] IController ResolveByView([NotNull] IView view);

		bool IsViewHasController([NotNull] IView view);
	}
}
