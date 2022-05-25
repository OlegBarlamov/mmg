using FrameworkSDK.MonoGame.Basic;
using FrameworkSDK.MonoGame.Mvc;

namespace FrameworkSDK.MonoGame.Mvc
{
    public interface IController : IUpdatable, INamed, ISceneComponent
	{
		IView View { get; }

		object DataModel { get; }

	    bool IsOwnedDataModel(object model);

	    void SetDataModel(object dataModel);

	    void SetView(IView view);
    }
}
