using System;

namespace FrameworkSDK.MonoGame.GameStructure.Mapping
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

        internal IncompatibleControllerTypeException(string message, Exception inner, params object[] args)
            : this(string.Format(DefaultFormatProvider, message, args), inner)
        {
        }

        internal IncompatibleControllerTypeException(string message, params object[] args)
            : this(string.Format(DefaultFormatProvider, message, args))
        {
        }
    }
}
