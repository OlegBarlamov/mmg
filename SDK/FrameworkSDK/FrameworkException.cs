using System;

namespace FrameworkSDK
{
    public class FrameworkException : Exception
    {
        internal FrameworkException(string message)
            : base(message)
        {
        }

        internal FrameworkException(string message, Exception inner)
            : base(message, inner)
        {
        }

        internal FrameworkException(string message,  Exception inner, params object[] args)
            : this(string.Format(message, args), inner)
        {
        }

        internal FrameworkException(string message, params object[] args)
            : this(string.Format(message, args))
        {
        }
    }
}
