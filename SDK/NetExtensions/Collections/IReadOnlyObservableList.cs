using System;
using System.Collections.Generic;

namespace NetExtensions.Collections
{
    public interface IReadOnlyObservableList<out T> : IReadOnlyList<T>
    {
        event Action<T> ItemAdded;
        event Action<T> ItemRemoved;
    }
}