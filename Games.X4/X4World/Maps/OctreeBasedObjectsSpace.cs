using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGameExtensions.DataStructures;

namespace X4World.Maps
{
    public class OctreeBasedObjectsSpace : IObjectsSpace<Vector3, IWrappedDetails>
    {
        public int Count { get; private set; }
        public bool IsReadOnly { get; } = false;
        
        private readonly AutoSplitOctreeNode<IWrappedDetails> _octree;

        public OctreeBasedObjectsSpace(Vector3 center, float size, int elementsCountToSplitOctree)
        {
            _octree = new AutoSplitOctreeNode<IWrappedDetails>(center, size, elementsCountToSplitOctree);
        }
        
        public IEnumerator<IWrappedDetails> GetEnumerator()
        {
            return _octree.EnumerateAllLeafs().SelectMany(x => x.DataObjects).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(IWrappedDetails item)
        {
            if (!_octree.ContainsPoint(item.Position))
                throw new Exception("Item is out of octree");
            
            _octree.AddItem(item);
            Count++;
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public bool Contains(IWrappedDetails item)
        {
            if (item == null)
                return false;
            
            var leaf = _octree.GetLeafWithPoint(item.Position);
            return leaf != null && leaf.DataObjects.Contains(item);
        }

        public void CopyTo(IWrappedDetails[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(IWrappedDetails item)
        {
            throw new System.NotImplementedException();
        }
        
        public IReadOnlyCollection<IWrappedDetails> GetInRange(Vector3 start, Vector3 end)
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyCollection<IWrappedDetails> GetInRadius(Vector3 center, float radius)
        {
            return _octree.EnumerateLeafsInRadiusAroundPoint(center, radius)
                .SelectMany(x => x.DataObjects)
                .ToArray();
        }

        public bool ContainsPoint(Vector3 point)
        {
            return _octree.ContainsPoint(point);
        }
    }
}