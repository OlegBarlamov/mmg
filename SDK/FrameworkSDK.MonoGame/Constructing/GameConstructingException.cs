using System;

namespace FrameworkSDK.MonoGame.Constructing
{
    public class GameConstructingException : FrameworkMonoGameException
    {
        internal GameConstructingException(string message) : base(message)
        {
        }

        internal GameConstructingException(string message, Exception inner) : base(message, inner)
        {
        }

        internal GameConstructingException(string message, Exception inner, params object[] args)
            : this(string.Format(DefaultFormatProvider, message, args), inner)
        {
        }

        internal GameConstructingException(string message, params object[] args)
            : this(string.Format(DefaultFormatProvider, message, args))
        {
        }
    }
}
