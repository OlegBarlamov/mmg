using System;

namespace FrameworkSDK.Game.Scenes
{
	public class ScenesException : FrameworkException
	{
		public ScenesException()
		{
		}

		public ScenesException(string message)
			: base(message)
		{
		}

		public ScenesException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
