using FrameworkSDK.Game.Views;
using JetBrains.Annotations;

namespace FrameworkSDK.Game.Mapping
{
	public interface IViewResolver
	{
		[NotNull] IView ResolveByModel([NotNull] object model);

		bool IsModelHasView([NotNull] object model);

	    void Initialize();
    }
}
