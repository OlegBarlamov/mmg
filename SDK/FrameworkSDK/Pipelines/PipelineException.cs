using System;

namespace FrameworkSDK.Pipelines
{
    public class PipelineException : FrameworkException
    {
        internal PipelineException(string message)
            : base(message)
        {
        }

        internal PipelineException(string message, Exception inner)
            : base(message, inner)
        {
        }

        internal PipelineException(string message, Exception inner, params object[] args)
            : this(string.Format(message, args), inner)
        {
        }

        internal PipelineException(string message, params object[] args)
            : this(string.Format(message, args))
        {
        }
    }
}