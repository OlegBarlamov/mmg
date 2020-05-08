using System;
using FrameworkSDK.MonoGame.Basic;

namespace FrameworkSDK.MonoGame.Mvc
{
	internal interface IScenesController : IUpdateable, IDrawable, IDisposable
	{
		bool CanSceneChange { get; }

		IScene CurrentScene { get; set; }
	}
}
