using System;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.IoC
{
	public sealed class FrameworkIocException : FrameworkException
	{
		internal FrameworkIocException(string message)
			: base(message)
		{
		}

		internal FrameworkIocException(string message, Exception inner)
			: base(message, inner)
		{
		}

	    internal FrameworkIocException(string message, Exception inner, params object[] args)
	        : this(string.Format(message, args), inner)
	    {
	    }

	    internal FrameworkIocException(string message, params object[] args)
	        : this(string.Format(message, args))
	    {
	    }
    }
}
