using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace NetExtensions.Collections
{
    public class IndexedEnumerator<T> : IEnumerator<T>
    {
        public T Current => ByIndexProvider.Invoke(_index);
        
        private Func<int, T> ByIndexProvider { get; }
        private Func<int> CountProvider { get; }

        private int _index = -1;

        public IndexedEnumerator([NotNull] Func<int, T> byIndexProvider, [NotNull] Func<int> countProvider)
        {
            ByIndexProvider = byIndexProvider ?? throw new ArgumentNullException(nameof(byIndexProvider));
            CountProvider = countProvider ?? throw new ArgumentNullException(nameof(countProvider));
        }
        
        public bool MoveNext()
        {
            _index++;
            return _index < CountProvider.Invoke();
        }

        public void Reset()
        {
            _index = -1;
        }

        object IEnumerator.Current => Current;
        
        public void Dispose()
        {
            Reset();
        }
    }
}