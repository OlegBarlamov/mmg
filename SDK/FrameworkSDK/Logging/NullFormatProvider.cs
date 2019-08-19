using System;

namespace FrameworkSDK.Logging
{
    public class NullFormatProvider : IFormatProvider, ICustomFormatter
    {
        public static IFormatProvider Instance { get; } = new NullFormatProvider();

        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
            {
                return this;
            }

            return null;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg == null)
            {
                return "NULL";
            }

            if (arg is IFormattable formattable)
            {
                return formattable.ToString(format, formatProvider);
            }
            return arg.ToString();
        }
    }
}
