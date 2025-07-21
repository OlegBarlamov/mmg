using System;

namespace Epic.Core
{
    public class Pair<T> : Tuple<T, T>
    {
        public Pair(T item1, T item2) : base(item1, item2)
        {
        }
    }
}