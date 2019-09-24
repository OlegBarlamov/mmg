using System;
using FrameworkSDK.Logging;

namespace FrameworkSDK.MonoGame
{
    public class FrameworkMonoGameException : Exception
    {
        protected static IFormatProvider DefaultFormatProvider { get; } = new NullFormatProvider();

        internal FrameworkMonoGameException(string message)
            : base(message)
        {
        }

        internal FrameworkMonoGameException(string message, Exception inner)
            : base(message, inner)
        {
        }

        internal FrameworkMonoGameException(string message,  Exception inner, params object[] args)
            : this(string.Format(DefaultFormatProvider, message, args), inner)
        {
        }

        internal FrameworkMonoGameException(string message, params object[] args)
            : this(string.Format(DefaultFormatProvider, message, args))
        {
        }
    }
}
