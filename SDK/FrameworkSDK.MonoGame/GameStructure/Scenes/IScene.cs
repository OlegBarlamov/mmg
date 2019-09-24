
namespace FrameworkSDK.MonoGame.GameStructure.Scenes
{
	internal interface IScene : IControllersManager, IViewsManager, IUpdatable, IClosable, IDrawable, INamed
    {
        object Model { get; set; }

		void OnOpened();
	}
}
