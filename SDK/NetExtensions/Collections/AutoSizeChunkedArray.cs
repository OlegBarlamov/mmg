using System;
using System.Collections;
using System.Collections.Generic;

namespace NetExtensions.Collections
{
    public class AutoSizeChunkedArray<T> : IAutoSizeArray<T>
    {
        public T this[int index] => GetItem(index);

        public int Count => _count;
        private int _count;

        private readonly IEnumerator<T> _enumerator;
        private readonly int _chunkSize;
        private readonly List<T[]> _chunks = new List<T[]>();

        public AutoSizeChunkedArray(int chunkSize, int startChunksCount = 1)
        {
            if (chunkSize <= 0) throw new ArgumentOutOfRangeException(nameof(chunkSize));
            if (startChunksCount < 1) throw new ArgumentOutOfRangeException(nameof(startChunksCount));
            _chunkSize = chunkSize;
            _enumerator = new IndexedEnumerator<T>(GetItem, GetCount);
            for (int i = 0; i < startChunksCount; i++)
            {
                AddChunk();
            }
        }

        public void ResetIndex()
        {
            _count = 0;
            _enumerator.Reset();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            var chunkIndex = _count / _chunkSize;
            if (chunkIndex >= _chunks.Count)
            {
                chunkIndex = AddChunk();
            }

            _chunks[chunkIndex][_count % _chunkSize] = item;
            _count++;
        }

        private int AddChunk() 
        {
            _chunks.Add(new T[_chunkSize]);
            return _chunks.Count - 1;
        }

        private int GetCount()
        {
            return _count;
        }

        private T GetItem(int index)
        {
            var chunkIndex = index / _chunkSize;
            return _chunks[chunkIndex][index % _chunkSize];
        }
    }
}