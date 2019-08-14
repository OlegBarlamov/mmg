using System;
using JetBrains.Annotations;

namespace NetExtensions
{
    public interface IKeyValueHeap<in TKey, TValue>
    {
        bool IsEmpty { get; }

        TValue this[TKey key] { get; set; }

        TValue GetValue(TKey key);

        void SetValue(TKey key, TValue value);

        void Clear();
    }

    public static class KeyValueHeapExtensions
    {
        [CanBeNull]
        public static T GetObject<T>([NotNull] this IKeyValueHeap<string, object> keyObjectHeap, [NotNull] string key)
        {
            if (keyObjectHeap == null) throw new ArgumentNullException(nameof(keyObjectHeap));
            if (key == null) throw new ArgumentNullException(nameof(key));

            var @object = keyObjectHeap.GetValue(key);

            try
            {
                return (T)(object)@object;
            }
            catch (Exception)
            {
                return default(T);
            }
        }
    }
}
