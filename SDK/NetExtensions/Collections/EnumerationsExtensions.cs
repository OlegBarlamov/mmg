using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace NetExtensions.Collections
{
	public static class EnumerationsExtensions
	{
		public static void AddRange<T>(this List<T> list, params T[] items)
		{
			list.AddRange(items);
		}

	    public static string ArrayToString<T>([NotNull] this IReadOnlyCollection<T> collection)
	    {
	        if (collection == null) throw new ArgumentNullException(nameof(collection));
	        return collection.ArrayToString("{0}", ", ");
	    }

	    public static string ArrayToString<T>([NotNull] this IReadOnlyCollection<T> collection, string format)
	    {
	        if (collection == null) throw new ArgumentNullException(nameof(collection));
	        return collection.ArrayToString(format, ", ");
	    }

	    public static string ArrayToString<T>([NotNull] this IReadOnlyCollection<T> collection, string format, string separator)
	    {
	        if (collection == null) throw new ArgumentNullException(nameof(collection));
	        return String.Join(separator, collection.Select(item => String.Format(format, item)));
	    }
	}
}
