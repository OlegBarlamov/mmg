using FrameworkSDK.Game.Scenes;
using FrameworkSDK.Game.Views;

namespace FrameworkSDK.Game.Controllers
{
    public interface IController : IUpdatable, INamed, ISceneComponent
	{
		IView View { get; }

		object Model { get; }

	    bool IsOwnedModel(object model);

	    void SetModel(object dataModel);

	    void SetView(IView view);
    }
}
