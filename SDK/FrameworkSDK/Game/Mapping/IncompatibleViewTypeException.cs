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

        internal IncompatibleViewTypeException(string message, Exception inner, params object[] args)
            : this(string.Format(message, args), inner)
        {
        }

        internal IncompatibleViewTypeException(string message, params object[] args)
            : this(string.Format(message, args))
        {
        }
    }
}
