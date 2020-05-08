using System;
using System.Collections;
using System.Collections.Generic;

namespace FrameworkSDK.MonoGame.Basic
{
    public class ObservableList<T> : IReadOnlyObservableList<T>, IList<T>
    {
        public event Action<T> ItemAdded;
        public event Action<T> ItemRemoved;

        public int Count => _list.Count;
        public bool IsReadOnly => ((IList<T>) _list).IsReadOnly;
        
        public T this[int index]
        {
            get => _list[index];
            set
            {
                var item = _list[index];
                _list[index] = value;
                ItemRemoved?.Invoke(item);
                ItemAdded?.Invoke(value);
            }
        }
        
        private readonly List<T> _list = new List<T>();
        
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_list).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }
        
        public void Add(T item)
        {
            _list.Add(item);
            ItemAdded?.Invoke(item);
        }

        public void Clear()
        {
            var items = _list.ToArray();
            _list.Clear();
            foreach (var item in items)
            {
                ItemRemoved?.Invoke(item);
            }
        }
        
        public bool Remove(T item)
        {
            var removed = _list.Remove(item); 
            if (removed)
            {
                ItemRemoved?.Invoke(item);
            }
            return removed;
        }
        
        public void Insert(int index, T item)
        {
            _list.Insert(index, item);
            ItemAdded?.Invoke(item);
        }

        public void RemoveAt(int index)
        {
            var item = _list[index];
            Remove(item);
        }
    }
}