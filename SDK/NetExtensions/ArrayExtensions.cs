using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace NetExtensions
{
	public static class ArrayExtensions
	{
		public static void For<T>(this T[] array, [NotNull] Func<T, int, bool> action)
		{
			if (action == null) throw new ArgumentNullException(nameof(action));
			for (int index = 0; index < array.Length; index++)
			{
				if (action(array[index], index))
					return;
			}
		}

		public static void For<T>(this T[,] array, [NotNull] Func<T, int, int, bool> action)
		{
			if (action == null) throw new ArgumentNullException(nameof(action));
			var n = array.GetLength(0);
			var m = array.GetLength(1);
			for (int i = 0; i < n; i++)
			for (int j = 0; j < m; j++)
			{
				if (action(array[i, j], i, j))
					return;
			}
		}

		public static TOut[,] Select<TIn, TOut>(this TIn[,] array, [NotNull] Func<TIn, int, int, TOut> factory)
		{
			if (factory == null) throw new ArgumentNullException(nameof(factory));

			var n = array.GetLength(0);
			var m = array.GetLength(1);

			var result = new TOut[n, m];

			for (int i = 0; i < n; i++)
			for (int j = 0; j < m; j++)
			{
				result[i, j] = factory(array[i, j], i, j);
			}

			return result;
		}

		public static string ArrayToString<T>([NotNull] this IReadOnlyCollection<T> collection)
		{
			if (collection == null) throw new ArgumentNullException(nameof(collection));
			return collection.ArrayToString(", ", "{0}");
		}

		public static string ArrayToString<T>([NotNull] this IReadOnlyCollection<T> collection, string separator)
		{
			if (collection == null) throw new ArgumentNullException(nameof(collection));
			return collection.ArrayToString(separator, "{0}");
		}

		public static string ArrayToString<T>([NotNull] this IReadOnlyCollection<T> collection, string separator, string format)
		{
			if (collection == null) throw new ArgumentNullException(nameof(collection));
			return string.Join(separator, collection.Select(item => string.Format(format, item)));
		}
	}
}