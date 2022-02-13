using System;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.DependencyInjection
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
	        : this(string.Format(DefaultFormatProvider, message, args), inner)
	    {
	    }

	    internal FrameworkIocException(string message, params object[] args)
	        : this(string.Format(DefaultFormatProvider, message, args))
	    {
	    }
    }
}
