using System;

namespace FrameworkSDK.Game.Mapping
{
    public class IncompatibleViewTypeException : MappingException
    {
        public IncompatibleViewTypeException()
        {
        }

        public IncompatibleViewTypeException(string message)
            : base(message)
        {
        }

        public IncompatibleViewTypeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
