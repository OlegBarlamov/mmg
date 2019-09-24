using System;
using FrameworkSDK.MonoGame.GameStructure.Scenes;

namespace FrameworkSDK.MonoGame.GameStructure
{
	internal interface IScenesController : IUpdatable, IDrawableComponent, IDisposable
	{
		bool CanSceneChange { get; }

		IScene CurrentScene { get; set; }
	}
}
