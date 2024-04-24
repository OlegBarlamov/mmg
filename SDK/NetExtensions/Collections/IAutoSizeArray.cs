using System.Collections.Generic;

namespace NetExtensions.Collections
{
    public interface IAutoSizeArray<T> : IReadOnlyList<T>
    {
        void ResetIndex();

        void Add(T item);
    }
}