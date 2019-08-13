using System;

namespace FrameworkSDK.Game.Mapping
{
    public class MappingException : FrameworkException
    {
	    internal MappingException(string message)
            : base(message)
        {
        }

	    internal MappingException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}