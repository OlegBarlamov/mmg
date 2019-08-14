using System;

namespace FrameworkSDK.Game.Mapping
{
    public class IncompatibleControllerTypeException : MappingException
    {
	    internal IncompatibleControllerTypeException(string message)
            : base(message)
        {
        }

	    internal IncompatibleControllerTypeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
