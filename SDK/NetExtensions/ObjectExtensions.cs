using System;
using JetBrains.Annotations;

namespace NetExtensions
{
    public static class ObjectExtensions
    {
        public static string GetTypeName([NotNull] this object @object)
        {
            if (@object == null) throw new ArgumentNullException(nameof(@object));
            return @object.GetType().Name;
        }
    }
}