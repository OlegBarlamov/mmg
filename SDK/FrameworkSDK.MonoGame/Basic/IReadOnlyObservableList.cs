using System;
using System.Collections.Generic;

namespace FrameworkSDK.MonoGame.Basic
{
    public interface IReadOnlyObservableList<out T> : IReadOnlyList<T>
    {
        event Action<T> ItemAdded;
        event Action<T> ItemRemoved;
    }
}