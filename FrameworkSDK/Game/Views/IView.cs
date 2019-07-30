using FrameworkSDK.Game.Scenes;

namespace FrameworkSDK.Game.Views
{
	public interface IView : IDrawable, INamed, ISceneComponent
	{
		void Destroy();
	}
}
