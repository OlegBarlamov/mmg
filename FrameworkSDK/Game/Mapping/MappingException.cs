using System;

namespace FrameworkSDK.Game.Mapping
{
    public class MappingException : FrameworkException
    {
        public MappingException()
        {
        }

        public MappingException(string message)
            : base(message)
        {
        }

        public MappingException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}