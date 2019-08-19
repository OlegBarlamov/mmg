using FrameworkSDK.Game.Controllers;
using FrameworkSDK.Game.Scenes;

namespace FrameworkSDK.Game.Views
{
	public interface IView : IDrawable, INamed, ISceneComponent
    {
	    object DataModel { get; }

        IController Controller { get; }

        void Destroy();

	    void SetDataModel(object dataModel);

	    void SetController(IController controller);
	}
}
