using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace MonoGameExtensions
{
	public class UpdatableCollection<T> : ICollection<T>
	{
		public int Count => _clearAll ? 0 : ActiveItems.Count + _itemsToAdd.Count - _itemsToRemove.Count;

		public bool IsReadOnly => false;

		[NotNull]
		private readonly List<T> _itemsToAdd = new List<T>();
		[NotNull]
		private readonly List<T> _itemsToRemove = new List<T>();
		[NotNull] private List<T> ActiveItems { get; }

		private bool _clearAll;

		public void Update()
		{
			if (_clearAll)
			{
				ActiveItems.Clear();
			}
			else
			{
				ActiveItems.AddRange(_itemsToAdd);
				foreach (var toRemove in _itemsToRemove)
					ActiveItems.Remove(toRemove);
			}

			_itemsToAdd.Clear();
			_itemsToRemove.Clear();
			_clearAll = false;
		}

		public UpdatableCollection()
		{
			ActiveItems = new List<T>();
		}

		public UpdatableCollection(IEnumerable<T> items)
		{
			ActiveItems = new List<T>(items);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return ActiveItems.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(T item)
		{
			_itemsToAdd.Add(item);
		}

		public void Clear()
		{
			_clearAll = true;
		}

	    public IReadOnlyCollection<T> GetAllWithToAddItems()
	    {
            if (_clearAll)
                return new T[0];

	        return new List<T>(ActiveItems
	            .Concat(_itemsToAdd)
	            .Except(_itemsToRemove));
	    }


        public bool Contains(T item)
		{
			return !_clearAll && ActiveItems
				       .Concat(_itemsToAdd)
				       .Except(_itemsToRemove)
				       .Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public bool Remove(T item)
		{
			_itemsToRemove.Add(item);
			return true;
		}

		public T Find([NotNull] Func<T, bool> predicate)
		{
			return GetAllWithToAddItems().FirstOrDefault(predicate);
		}
		
	}
}
