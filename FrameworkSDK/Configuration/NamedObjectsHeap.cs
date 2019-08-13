using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace FrameworkSDK.Configuration
{
    public class NamedObjectsHeap
    {
        [CanBeNull]
        public object this[[NotNull] string key] => GetObject(key);

        public bool IsEmpty => _heap.Count == 0;

        private readonly Dictionary<string, object> _heap = new Dictionary<string, object>();

        [CanBeNull]
        public object GetObject([NotNull] string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            return _heap.TryGetValue(key, out var result) ? result : null;
        }

        public void SetObject([NotNull] string key, [NotNull] object @object)
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
