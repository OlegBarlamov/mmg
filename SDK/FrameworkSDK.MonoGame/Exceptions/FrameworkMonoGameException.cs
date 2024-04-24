using System;
using FrameworkSDK.Logging;

namespace FrameworkSDK.MonoGame
{
    public class FrameworkMonoGameException : FrameworkException
    {
        protected static IFormatProvider DefaultFormatProvider { get; } = new NullFormatProvider();

        protected internal FrameworkMonoGameException(string message)
            : base(message)
        {
        }

        protected internal FrameworkMonoGameException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected internal FrameworkMonoGameException(string message,  Exception inner, params object[] args)
            : this(string.Format(DefaultFormatProvider, message, args), inner)
        {
        }

        protected internal FrameworkMonoGameException(string message, params object[] args)
            : this(string.Format(DefaultFormatProvider, message, args))
        {
        }
    }
}
