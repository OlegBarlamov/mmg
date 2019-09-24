using FrameworkSDK.MonoGame.GameStructure.Controllers;
using FrameworkSDK.MonoGame.GameStructure.Scenes;

namespace FrameworkSDK.MonoGame.GameStructure.Views
{
	public interface IView : IGraphicComponent, ISceneComponent, INamed
    {
	    object DataModel { get; }

        IController Controller { get; }

        void Destroy();

	    void SetDataModel(object dataModel);

	    void SetController(IController controller);
	}
}
