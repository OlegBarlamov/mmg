using System;

namespace FrameworkSDK.Game.Scenes
{
	public class ScenesException : FrameworkException
	{
		internal ScenesException(string message)
			: base(message)
		{
		}

		internal ScenesException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
