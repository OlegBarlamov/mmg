using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGameExtensions.DataStructures
{
    public class AutoSplitOctreeNode<TData> : OctreeNodeBase<TData> where TData : ILocatable3D
    {
        public int MaxElementsCount { get; }
        public bool Subdivided => !Children.IsEmpty;
        
        private readonly ConcurrentQueue<TData> _items = new ConcurrentQueue<TData>();
        
        public AutoSplitOctreeNode(Vector3 center, float size, int elementsCountToSplit)
            : this(null, center, size, 0, elementsCountToSplit)
        {
        }

        private AutoSplitOctreeNode(AutoSplitOctreeNode<TData> parent, Vector3 center, float size, int level,
            int maxElementsCount) : base(parent, center, size, level)
        {
            if (maxElementsCount <= 0) throw new ArgumentOutOfRangeException(nameof(maxElementsCount));
            
            MaxElementsCount = maxElementsCount;
            DataObjects = _items;
        }

        public void AddItem(TData item)
        {
            if (Subdivided)
            {
                AddItemToChildren(item);
            }
            else
            {
                _items.Enqueue(item);
                if (_items.Count > MaxElementsCount)
                {
                    Split();
                }
            }
        }

        private void Split()
        {
            base.Split(NodeFactory);

            while (_items.TryDequeue(out var item))
            {
                AddItemToChildren(item);
            }
        }

        private static IOctreeNode<TData> NodeFactory(IOctreeNode<TData> parentnode, Vector3 center, float size, int level)
        {
            var parent = (AutoSplitOctreeNode<TData>)parentnode;
            return new AutoSplitOctreeNode<TData>(parent, center, size, level, parent.MaxElementsCount);
        }

        private void AddItemToChildren(TData item)
        {
            var targetChild = (AutoSplitOctreeNode<TData>)this.GetChildContainsPoint(item.Position);
            targetChild.AddItem(item);
        }
    }
}