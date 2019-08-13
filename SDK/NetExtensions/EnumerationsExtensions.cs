using System.Collections.Generic;

namespace NetExtensions
{
	public static class EnumerationsExtensions
	{
		public static void AddRange<T>(this List<T> list, params T[] items)
		{
			list.AddRange(items);
		}
	}
}
