using System;
using System.Collections.Generic;

namespace Epic.Logic.Utils
{
    internal static class BinarySearch
    {
        public static int FindClosestNotExceedingIndex<T>(IReadOnlyList<T> sortedArray, Func<T, int> func, int target)
        {
            int left = 0;
            int right = sortedArray.Count - 1;
            int resultIndex = -1; // -1 means no suitable index found

            while (left <= right)
            {
                int mid = left + (right - left) / 2;
                int midValue = func(sortedArray[mid]);

                if (midValue <= target)
                {
                    resultIndex = mid; // store index of valid candidate
                    left = mid + 1;    // keep searching right for last match
                }
                else
                {
                    right = mid - 1;
                }
            }

            return resultIndex;
        }
    }
}