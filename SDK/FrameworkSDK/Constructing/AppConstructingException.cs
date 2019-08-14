using System;

namespace FrameworkSDK.Constructing
{
	public class AppConstructingException : FrameworkException
	{
		internal AppConstructingException(string message) : base(message)
		{
		}

		internal AppConstructingException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}
