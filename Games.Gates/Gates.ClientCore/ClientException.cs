using System;
using FrameworkSDK.Logging;

namespace Gates.ClientCore
{
    public class ClientException : Exception
    {
        protected static IFormatProvider DefaultFormatProvider { get; } = new NullFormatProvider();

        internal ClientException(string message)
            : base(message)
        {
        }

        internal ClientException(string message, Exception inner)
            : base(message, inner)
        {
        }

        internal ClientException(string message, Exception inner, params object[] args)
            : this(string.Format(DefaultFormatProvider, message, args), inner)
        {
        }

        internal ClientException(string message, params object[] args)
            : this(string.Format(DefaultFormatProvider, message, args))
        {
        }
    }
}
