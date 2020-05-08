using System;
using FrameworkSDK.Localization;

namespace FrameworkSDK.Common
{
    public class ObjectInitializedException : FrameworkException
    {
        public ObjectInitializedException(string message) : base(ToMessage(message))
        {
        }

        public ObjectInitializedException(string message, Exception inner) : base(ToMessage(message), inner)
        {
        }

        public ObjectInitializedException(string message, Exception inner, params object[] args) : base(ToMessage(message), inner, args)
        {
        }

        public ObjectInitializedException(string message, params object[] args) : base(ToMessage(message), args)
        {
        }

        private static string ToMessage(string baseMessage)
        {
            return string.Format(Strings.Exceptions.ObjectAlreadyInitialized, baseMessage);
        } 
    }
}