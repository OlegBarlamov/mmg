using System;
using JetBrains.Annotations;

namespace NetExtensions.Collections
{
    public static class EnumerableExtended
    {
        public static void For(int start, int end, [NotNull] Action<int> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            for (int i = start; i < end; i++)
            {
                action(i);
            }
        } 
        
        public static void For(int iEnd, int jEnd, int kEnd, [NotNull] Action<int, int, int> action)
        {
            For(0, iEnd, 0, jEnd, 0, kEnd, action);
        }
        
        public static void For(int iStart, int iEnd, int jStart, int jEnd, int kStart, int kEnd, [NotNull] Action<int, int, int> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            
            for (int i = iStart; i < iEnd; i++)
            {
                for (int j = jStart; j < jEnd; j++)
                {
                    for (int k = kStart; k < kEnd; k++)
                        action(i, j, k);
                }
            }
        }
    }
}