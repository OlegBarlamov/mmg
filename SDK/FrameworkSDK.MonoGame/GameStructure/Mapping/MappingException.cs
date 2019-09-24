using System;

namespace FrameworkSDK.MonoGame.GameStructure.Mapping
{
    public class MappingException : FrameworkMonoGameException
    {
	    internal MappingException(string message)
            : base(message)
        {
        }

	    internal MappingException(string message, Exception inner)
            : base(message, inner)
        {
        }

        internal MappingException(string message, Exception inner, params object[] args)
            : this(string.Format(DefaultFormatProvider, message, args), inner)
        {
        }

        internal MappingException(string message, params object[] args)
            : this(string.Format(DefaultFormatProvider, message, args))
        {
        }
    }
}