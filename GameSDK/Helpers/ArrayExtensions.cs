using System;
using JetBrains.Annotations;

namespace GameSDK.Helpers
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

			var result = new TOut[n,m];

			for (int i = 0; i < n; i++)
			for (int j = 0; j < m; j++)
			{
				result[i, j] = factory(array[i, j], i, j);
			}

			return result;
		}
	}
}
