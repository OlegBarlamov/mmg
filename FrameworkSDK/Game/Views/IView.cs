using FrameworkSDK.Game.Controllers;
using FrameworkSDK.Game.Scenes;

namespace FrameworkSDK.Game.Views
{
	public interface IView : IDrawable, INamed, ISceneComponent
	{
	    object DataModel { get; set; }

        IController Controller { get; set; }

	    void SetOwner(Scene ownedScene);

        void Destroy();
	}
}
