using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK
{
    public interface INamed
    {
        string Name { get; }
    }

    public static class NamedExtensions
    {
        public static bool ContainsWithName(this IEnumerable<INamed> enumeration, string name)
        {
            return enumeration.Any(named => named.Name == name);
        }

        public static bool ContainsWithName(this IEnumerable<INamed> enumeration, string name, StringComparison stringComparison)
        {
            return enumeration.Any(named => string.Compare(named.Name, name, stringComparison) == 0);
        }
    }
}
