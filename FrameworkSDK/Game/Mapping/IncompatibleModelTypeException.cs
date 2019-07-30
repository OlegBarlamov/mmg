using System;

namespace FrameworkSDK.Game.Mapping
{
    public class IncompatibleModelTypeException : MappingException
    {
        public IncompatibleModelTypeException()
        {
        }

        public IncompatibleModelTypeException(string message)
            : base(message)
        {
        }

        public IncompatibleModelTypeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
