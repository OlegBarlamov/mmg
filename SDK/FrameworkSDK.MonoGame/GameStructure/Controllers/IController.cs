using FrameworkSDK.MonoGame.GameStructure.Scenes;
using FrameworkSDK.MonoGame.GameStructure.Views;

namespace FrameworkSDK.MonoGame.GameStructure.Controllers
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
