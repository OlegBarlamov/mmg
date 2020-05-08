using System;
using FrameworkSDK.Localization;

namespace FrameworkSDK.Common
{
    public class ObjectNotInitializedException : FrameworkException
    {
        public ObjectNotInitializedException(string message) : base(ToMessage(message))
        {
        }

        public ObjectNotInitializedException(string message, Exception inner) : base(ToMessage(message), inner)
        {
        }

        public ObjectNotInitializedException(string message, Exception inner, params object[] args) : base(ToMessage(message), inner, args)
        {
        }

        public ObjectNotInitializedException(string message, params object[] args) : base(ToMessage(message), args)
        {
        }

        private static string ToMessage(string baseMessage)
        {
            return string.Format(Strings.Exceptions.ObjectNotInitialized, baseMessage);
        } 
    }
}