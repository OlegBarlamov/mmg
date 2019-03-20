using System;

namespace GameSDK
{
	public class GameSDKException : Exception
	{
		public GameSDKException()
		{
		}

		public GameSDKException(string message)
			: base(message)
		{
		}

		public GameSDKException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
