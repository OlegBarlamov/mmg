using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace MonoGameExtensions
{
	public class UpdatableCollection<T> : ICollection<T>
	{
		public int Count => _clearAll ? 0 : ActiveControllers.Count + _controllersToAdd.Count - _controllersToRemove.Count;

		public bool IsReadOnly => false;

		[NotNull]
		private readonly List<T> _controllersToAdd = new List<T>();
		[NotNull]
		private readonly List<T> _controllersToRemove = new List<T>();
		[NotNull] private List<T> ActiveControllers { get; }

		private bool _clearAll;

		public void Update()
		{
			if (_clearAll)
			{
				ActiveControllers.Clear();
			}
			else
			{
				ActiveControllers.AddRange(_controllersToAdd);
				foreach (var toRemove in _controllersToRemove)
					ActiveControllers.Remove(toRemove);
			}

			_controllersToAdd.Clear();
			_controllersToRemove.Clear();
			_clearAll = false;
		}

		public UpdatableCollection()
		{
			ActiveControllers = new List<T>();
		}

		public UpdatableCollection(IEnumerable<T> items)
		{
			ActiveControllers = new List<T>(items);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return ActiveControllers.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(T item)
		{
			_controllersToAdd.Add(item);
		}

		public void Clear()
		{
			_clearAll = true;
		}

	    public IReadOnlyList<T> GetAllWithToAddItems()
	    {
            if (_clearAll)
                return new T[0];

	        return new List<T>(ActiveControllers
	            .Concat(_controllersToAdd)
	            .Except(_controllersToRemove));
	    }


        public bool Contains(T item)
		{
			return !_clearAll && ActiveControllers
				       .Concat(_controllersToAdd)
				       .Except(_controllersToRemove)
				       .Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public bool Remove(T item)
		{
			_controllersToRemove.Add(item);
			return true;
		}
	}
}
