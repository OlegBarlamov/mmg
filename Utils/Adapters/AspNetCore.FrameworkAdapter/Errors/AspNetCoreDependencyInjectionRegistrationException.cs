using System;

namespace AspNetCore.FrameworkAdapter
{
    public class AspNetCoreDependencyInjectionRegistrationException : InvalidOperationException
    {
        public AspNetCoreDependencyInjectionRegistrationException(string message)
            : base(message)
        {
        }
    }
}