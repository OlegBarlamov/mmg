
namespace FrameworkSDK.Game.Scenes
{
	internal interface IScene : IControllersManager, IViewsManager, IUpdatable, IClosable, IDrawable, INamed
	{
		void OnOpened();
	}
}
