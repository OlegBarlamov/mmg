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

	    internal AppConstructingException(string message, Exception inner, params object[] args)
	        : this(string.Format(DefaultFormatProvider, message, args), inner)
	    {
	    }

	    internal AppConstructingException(string message, params object[] args)
	        : this(string.Format(DefaultFormatProvider, message, args))
	    {
	    }
    }
}
