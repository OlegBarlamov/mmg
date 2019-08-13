using System;

namespace FrameworkSDK.Configuration
{
    public class ConfigurationException : FrameworkException
    {
        internal ConfigurationException(string message)
            : base(message)
        {
        }

        internal ConfigurationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
