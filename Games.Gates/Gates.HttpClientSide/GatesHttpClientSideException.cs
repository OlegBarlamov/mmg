using System;

namespace Gates.HttpClientSide
{
    public class GatesHttpClientSideException : Exception
    {
        internal GatesHttpClientSideException(string message)
            : base(message)
        {
        }

        internal GatesHttpClientSideException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
