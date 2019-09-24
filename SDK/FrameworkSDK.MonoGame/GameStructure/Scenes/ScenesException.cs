using System;

namespace FrameworkSDK.MonoGame.GameStructure.Scenes
{
	public class ScenesException : FrameworkMonoGameException
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
	        : this(string.Format(DefaultFormatProvider, message, args), inner)
	    {
	    }

	    internal ScenesException(string message, params object[] args)
	        : this(string.Format(DefaultFormatProvider, message, args))
	    {
	    }
    }
}
