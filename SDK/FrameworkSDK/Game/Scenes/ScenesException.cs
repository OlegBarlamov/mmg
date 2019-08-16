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

	    internal ScenesException(string message, Exception inner, params object[] args)
	        : this(string.Format(message, args), inner)
	    {
	    }

	    internal ScenesException(string message, params object[] args)
	        : this(string.Format(message, args))
	    {
	    }
    }
}
