using FrameworkSDK.Game.Scenes;
using FrameworkSDK.Game.Views;

namespace FrameworkSDK.Game.Controllers
{
    public interface IController : IUpdatable, INamed, ISceneComponent
	{
		IView View { get; set; }

		object Model { get; set; }

	    bool IsOwnedModel(object model);

	    void SetOwner(Scene ownedScene);
	}
}
