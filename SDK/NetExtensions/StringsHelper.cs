using System;
using JetBrains.Annotations;

namespace NetExtensions
{
    public static class StringsHelper
    {
        public static string TrimStart([NotNull] this string targetString, [NotNull] string str, StringComparison stringComparison = StringComparison.CurrentCulture)
        {
            if (targetString == null) throw new ArgumentNullException(nameof(targetString));
            if (str == null) throw new ArgumentNullException(nameof(str));

            if (!targetString.StartsWith(str, stringComparison))
                return targetString;

            return targetString.Substring(str.Length);
        }

        public static string TrimEnd([NotNull] this string targetString, [NotNull] string str, StringComparison stringComparison = StringComparison.CurrentCulture)
        {
            if (targetString == null) throw new ArgumentNullException(nameof(targetString));
            if (str == null) throw new ArgumentNullException(nameof(str));

            if (!targetString.EndsWith(str, stringComparison))
                return targetString;

            return targetString.Substring(0, targetString.Length - str.Length);
        }

        public static string Trim([NotNull] this string targetString, [NotNull] string str, StringComparison stringComparison = StringComparison.CurrentCulture)
        {
            if (targetString == null) throw new ArgumentNullException(nameof(targetString));
            if (str == null) throw new ArgumentNullException(nameof(str));

            targetString = targetString.TrimStart(str, stringComparison);
            return targetString.TrimEnd(str, stringComparison);
        }
    }
}