using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NetExtensions;

namespace MonoGameExtensions
{
	public class ManualUpdatableCollection<T> : UpdatableCollection<T>
	{
		private bool _updateLocked = false;
		
		public ManualUpdatableCollection(IEnumerable<T> items)
			: base(items)
		{
			
		}

		public ManualUpdatableCollection()
		{
			
		}

		public override void Add(T item)
		{
			if (_updateLocked) 
				base.Add(item);
			else 
				ActiveItems.Add(item);
		}

		public override void Clear()
		{
			if (_updateLocked)
				base.Clear();
			else
				ActiveItems.Clear();
		}

		public override bool Remove(T item)
		{
			if (_updateLocked)
				return base.Remove(item);
			return ActiveItems.Remove(item);
		}
		

		public void UpdateStarted()
		{
			_updateLocked = true;
		}

		public void UpdateFinished(bool updateCollections)
		{
			_updateLocked = false;
			
			if (updateCollections)
				Update();
		}
	}
}
