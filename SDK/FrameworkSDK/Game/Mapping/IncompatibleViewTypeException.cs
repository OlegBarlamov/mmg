using System;

namespace FrameworkSDK.Game.Mapping
{
    public class IncompatibleViewTypeException : MappingException
    {
	    internal IncompatibleViewTypeException(string message)
            : base(message)
        {
        }

	    internal IncompatibleViewTypeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
