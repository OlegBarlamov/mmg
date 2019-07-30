using System;

namespace FrameworkSDK.Game.Mapping
{
    public class IncompatibleControllerTypeException : MappingException
    {
        public IncompatibleControllerTypeException()
        {
        }

        public IncompatibleControllerTypeException(string message)
            : base(message)
        {
        }

        public IncompatibleControllerTypeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
