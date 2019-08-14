
namespace FrameworkSDK.Game.Scenes
{
	internal interface IScene : IControllersManager, IUpdatable, IClosable, IDrawable, INamed
	{
		void OnOpened();
	}
}
