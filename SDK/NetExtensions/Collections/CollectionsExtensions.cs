using System.Collections.Generic;

namespace NetExtensions.Collections
{
    public static class CollectionsExtensions
    {
        public static void RemoveRange<T>(this ICollection<T> list, IEnumerable<T> itemsToBeRemoved)
        {
            foreach (var itemToBeRemoved in itemsToBeRemoved)
            {
                list.Remove(itemToBeRemoved);
            }
        }

        public static void SwapAndPopRemove<T>(this List<T> list, T item)
        {
            var idx = list.IndexOf(item);
            if (idx < 0) return;
            var last = list.Count - 1;
            list[idx] = list[last];
            list.RemoveAt(last);
        }
    }
}