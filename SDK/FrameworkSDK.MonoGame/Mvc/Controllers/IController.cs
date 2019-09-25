using FrameworkSDK.MonoGame.Mvc;

namespace FrameworkSDK.MonoGame.Mvc
{
    public interface IController : IUpdateable, INamed, ISceneComponent
	{
		IView View { get; }

		object Model { get; }

	    bool IsOwnedModel(object model);

	    void SetModel(object dataModel);

	    void SetView(IView view);
    }
}
