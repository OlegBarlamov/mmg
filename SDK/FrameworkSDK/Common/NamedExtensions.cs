using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK
{
    public static class NamedExtensions
    {
        public static bool ContainsByName([NotNull] this IEnumerable<INamed> enumeration, string name)
        {
            if (enumeration == null) throw new ArgumentNullException(nameof(enumeration));
            return enumeration.Any(named => named.Name == name);
        }

        public static bool ContainsByName([NotNull] this IEnumerable<INamed> enumeration, string name, StringComparison stringComparison)
        {
            if (enumeration == null) throw new ArgumentNullException(nameof(enumeration));
            return enumeration.Any(named => string.Compare(named.Name, name, stringComparison) == 0);
        }

        public static T FindByName<T>(this IEnumerable<T> enumeration, string name) where T : INamed
        {
            return enumeration.FirstOrDefault(named => named.Name == name);
        }

        public static T FindByName<T>([NotNull] this IEnumerable<T> enumeration, string name, StringComparison stringComparison) where T : INamed
        {
            if (enumeration == null) throw new ArgumentNullException(nameof(enumeration));
            return enumeration.FirstOrDefault(named => string.Compare(named.Name, name, stringComparison) == 0);
        }
    }
}