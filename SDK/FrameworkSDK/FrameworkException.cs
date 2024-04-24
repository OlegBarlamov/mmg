using System;
using FrameworkSDK.Logging;

namespace FrameworkSDK
{
    public class FrameworkException : Exception
    {
        protected static IFormatProvider DefaultFormatProvider { get; } = new NullFormatProvider();

        protected internal FrameworkException(string message)
            : base(message)
        {
        }

        protected internal FrameworkException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected internal FrameworkException(string message,  Exception inner, params object[] args)
            : this(string.Format(DefaultFormatProvider, message, args), inner)
        {
        }

        protected internal FrameworkException(string message, params object[] args)
            : this(string.Format(DefaultFormatProvider, message, args))
        {
        }
    }
}
