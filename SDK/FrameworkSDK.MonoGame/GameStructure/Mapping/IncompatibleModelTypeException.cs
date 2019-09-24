using System;

namespace FrameworkSDK.MonoGame.GameStructure.Mapping
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

        internal IncompatibleModelTypeException(string message, Exception inner, params object[] args)
            : this(string.Format(DefaultFormatProvider, message, args), inner)
        {
        }

        internal IncompatibleModelTypeException(string message, params object[] args)
            : this(string.Format(DefaultFormatProvider, message, args))
        {
        }
    }
}
