using System;
using FrameworkSDK.Game.Scenes;

namespace FrameworkSDK.Game
{
	internal interface IScenesController : IUpdatable, IDrawable, IDisposable
	{
		bool CanSceneChanged { get; }

		IScene CurrentScene { get; set; }
	}
}
