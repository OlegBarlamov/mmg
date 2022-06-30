using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace MonoGameExtensions.DataStructures
{
    public abstract class OctreeNodeBase<TData> : IOctreeNode<TData>
    {
        public event Action<IOctreeNode<TData>> NodeSubdivided; 
        public Vector3 Center { get; }
        /// <summary>
        /// Width/Height/Depth
        /// </summary>
        public float Size { get; }
        public int Level { get; }
        public BoundingBox BoundingBox { get; }
        public IReadOnlyCollection<TData> DataObjects { get; set; }
        public IOctreeNode<TData> Parent { get; }
        public OctreeChildrenNodes<TData> Children { get; private set; } = OctreeChildrenNodes<TData>.Empty;

        private bool _splitStarted;
        private readonly object _splitLocker = new object();
        
        public OctreeNodeBase(Vector3 center, float size): this(null, center, size, 0)
        {
            
        }
        
        protected OctreeNodeBase(IOctreeNode<TData> parent, Vector3 center, float size, int level)
        {
            Center = center;
            Size = size;
            Level = level;
            BoundingBox = new BoundingBox(center - new Vector3(size) / 2, center + new Vector3(size) / 2);
            Parent = parent;
        }

        public delegate IOctreeNode<TData> NodeFactory(IOctreeNode<TData> parentNode, Vector3 center, float size,
            int level);

        protected void Split([NotNull] NodeFactory factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            
            if (!Children.IsEmpty)
                throw new InvalidOperationException("Octree node already had been split");

            lock (_splitLocker)
            {
                if (_splitStarted)
                    return;

                _splitStarted = true;
            }
            
            Children = new OctreeChildrenNodes<TData>(
                factory(this, Center + new Vector3(-1, -1, -1) * Size / 4, Size / 2, Level + 1),
                factory(this, Center + new Vector3(1, -1, -1) * Size / 4, Size / 2, Level + 1),
                factory(this, Center + new Vector3(1, -1, 1) * Size / 4, Size / 2, Level + 1),
                factory(this, Center + new Vector3(-1, -1, 1) * Size / 4, Size / 2, Level + 1),
                factory(this, Center + new Vector3(-1, 1, -1) * Size / 4, Size / 2, Level + 1),
                factory(this, Center + new Vector3(1, 1, -1) * Size / 4, Size / 2, Level + 1),
                factory(this, Center + new Vector3(1, 1, 1) * Size / 4, Size / 2, Level + 1),
                factory(this, Center + new Vector3(-1, 1, 1) * Size / 4, Size / 2, Level + 1)
            );
            
            NodeSubdivided?.Invoke(this);
        }
    }
}