using System;
using System.Collections.Generic;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace NetExtensions.Collections
{
    public class NamedObjectsHeap<T> : IKeyValueHeap<string, T>
    {
        [CanBeNull]
        public T this[[NotNull] string key]
        {
            get => GetValue(key);
            // ReSharper disable once AssignNullToNotNullAttribute
            set => SetValue(key, value);
        }

        public bool IsEmpty => _heap.Count == 0;

        private readonly Dictionary<string, T> _heap = new Dictionary<string, T>();

        [CanBeNull]
        public T GetValue([NotNull] string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            return _heap.TryGetValue(key, out var result) ? result : default(T);
        }

        public void SetValue([NotNull] string key, [NotNull] T @object)
        {
            if (@object == null) throw new ArgumentNullException(nameof(@object));
            if (key == null) throw new ArgumentNullException(nameof(key));

            if (_heap.ContainsKey(key))
                _heap[key] = @object;
            else
                _heap.Add(key, @object);
        }

        public void Clear()
        {
            _heap.Clear();
        }
    }
}
