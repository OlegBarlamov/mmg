using System;

namespace FrameworkSDK.MonoGame
{
    public class ObjectNotInitializedException : FrameworkMonoGameException
    {
        internal ObjectNotInitializedException(string message)
            : base(message)
        {
        }

        internal ObjectNotInitializedException(string message, Exception inner)
            : base(message, inner)
        {
        }

        internal ObjectNotInitializedException(string message,  Exception inner, params object[] args)
            : this(string.Format(DefaultFormatProvider, message, args), inner)
        {
        }

        internal ObjectNotInitializedException(string message, params object[] args)
            : this(string.Format(DefaultFormatProvider, message, args))
        {
        }
    }
}
