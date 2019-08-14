using System;

namespace FrameworkSDK.Game.Mapping
{
    public class IncompatibleModelTypeException : MappingException
    {
	    internal IncompatibleModelTypeException(string message)
            : base(message)
        {
        }

	    internal IncompatibleModelTypeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
