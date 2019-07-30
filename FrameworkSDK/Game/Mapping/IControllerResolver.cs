using FrameworkSDK.Game.Controllers;
using JetBrains.Annotations;

namespace FrameworkSDK.Game.Mapping
{
	public interface IControllerResolver
	{
		[NotNull] IController ResolveByModel([NotNull] object model);

		bool IsModelHasController([NotNull] object model);

	    void Initialize();
	}
}
