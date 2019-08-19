using System;
using FrameworkSDK.Logging;

namespace FrameworkSDK
{
    public class FrameworkException : Exception
    {
        protected static IFormatProvider DefaultFormatProvider { get; } = new NullFormatProvider();

        internal FrameworkException(string message)
            : base(message)
        {
        }

        internal FrameworkException(string message, Exception inner)
            : base(message, inner)
        {
        }

        internal FrameworkException(string message,  Exception inner, params object[] args)
            : this(string.Format(DefaultFormatProvider, message, args), inner)
        {
        }

        internal FrameworkException(string message, params object[] args)
            : this(string.Format(DefaultFormatProvider, message, args))
        {
        }
    }
}
