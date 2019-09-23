using System;
using FrameworkSDK.Game.Scenes;

namespace FrameworkSDK.Game
{
	internal interface IScenesController : IUpdatable, IDrawableComponent, IDisposable
	{
		bool CanSceneChange { get; }

		IScene CurrentScene { get; set; }
	}
}
