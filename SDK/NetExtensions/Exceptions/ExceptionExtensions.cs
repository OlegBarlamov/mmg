using System;

namespace NetExtensions.Exceptions
{
    public static class ExceptionExtensions
    {
        public static T FindInnerException<T>(this Exception e, bool includingCurrent = false) where T : Exception
        {
            var exception = includingCurrent ? e : e.InnerException;
            while (exception != null)
            {
                if (exception is T desiredException)
                    return desiredException;
                
                exception = exception.InnerException;
            }
            return null;
        }
    }
}