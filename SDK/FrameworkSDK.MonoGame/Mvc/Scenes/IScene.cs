// ReSharper disable once CheckNamespace

using FrameworkSDK.MonoGame.Basic;

namespace FrameworkSDK.MonoGame.Mvc
{
	internal interface IScene : IControllersManager, IViewsManager, IUpdatable, IClosable, IDrawable, INamed
    {
        object Model { get; set; }

		void OnOpened();
	}
}
