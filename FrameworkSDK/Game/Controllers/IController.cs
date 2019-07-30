using FrameworkSDK.Game.Scenes;
using FrameworkSDK.Game.Views;

namespace FrameworkSDK.Game.Controllers
{
    public interface IController : IUpdatable, INamed, ISceneComponent
	{
		IView View { get; set; }

		object Model { get; }

	    bool IsOwnedModel(object model);
    }
}
