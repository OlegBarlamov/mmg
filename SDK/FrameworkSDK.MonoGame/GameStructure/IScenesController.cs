using System;
using FrameworkSDK.MonoGame.GameStructure.Scenes;

namespace FrameworkSDK.MonoGame.GameStructure
{
	internal interface IScenesController : IUpdatable, IDrawable, IDisposable
	{
		bool CanSceneChange { get; }

		IScene CurrentScene { get; set; }
	}
}
