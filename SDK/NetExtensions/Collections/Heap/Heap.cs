using System.Collections;
using System.Collections.Generic;

namespace NetExtensions.Collections
{
    public class Heap<T> : ICollection<T>, IReadOnlyCollection<T>
    {
        public int Count => _list.Count;
        public bool IsReadOnly => false;
        
        private readonly List<T> _list = new List<T>();
        private readonly Dictionary<T, int> _map = new Dictionary<T, int>();

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            _map.Add(item, _list.Count);
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
            _map.Clear();
        }

        public bool Contains(T item)
        {
            return _map.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(T item)
        {
            if (_map.TryGetValue(item, out var index))
            {
                var lastItem = _list[Count - 1];
                _list[index] = lastItem;
                _map[lastItem] = index;
                
                _list.RemoveAt(Count-1);
                _map.Remove(item);
                return true;
            }

            return false;
        }
    }
}