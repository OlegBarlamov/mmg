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
    }
}
