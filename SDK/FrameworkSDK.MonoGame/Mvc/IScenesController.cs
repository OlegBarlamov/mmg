using System;
using FrameworkSDK.MonoGame.Basic;

namespace FrameworkSDK.MonoGame.Mvc
{
	internal interface IScenesController : IUpdatable, IDisposable
	{
		bool CanSceneChange { get; }

		IScene CurrentScene { get; set; }
	}
}
