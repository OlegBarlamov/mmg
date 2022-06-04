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
    }
}