using System;
using FrameworkSDK.Logging;

namespace FrameworkSDK.MonoGame
{
    public class ObjectAlreadyInitializedException : FrameworkMonoGameException
    {
        internal ObjectAlreadyInitializedException(string message)
            : base(message)
        {
        }

        internal ObjectAlreadyInitializedException(string message, Exception inner)
            : base(message, inner)
        {
        }

        internal ObjectAlreadyInitializedException(string message,  Exception inner, params object[] args)
            : this(string.Format(DefaultFormatProvider, message, args), inner)
        {
        }

        internal ObjectAlreadyInitializedException(string message, params object[] args)
            : this(string.Format(DefaultFormatProvider, message, args))
        {
        }
    }
}
