using System;

namespace AspNetCore.FrameworkAdapter
{
    public class WrongHostBuildExecutionException : InvalidOperationException
    {
        internal WrongHostBuildExecutionException()
            : base("Don't call Build method on IAspNetCoreAppConfigurator interface. Use default Construct method instead.")
        {
        }
    }
}