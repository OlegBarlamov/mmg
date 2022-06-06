using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGameExtensions.DataStructures
{
    public class SimpleOctreeNode<TData> : OctreeNodeBase<TData>
    {
        public SimpleOctreeNode(Vector3 center, float size) : base(center, size)
        {
        }

        public SimpleOctreeNode(SimpleOctreeNode<TData> parent, Vector3 center, float size, int level) : base(parent, center, size, level)
        {
        }

        public void Split()
        {
            base.Split(NodeFactory);
        }

        private static IOctreeNode<TData> NodeFactory(IOctreeNode<TData> parentNode, Vector3 center, float size,
            int level)
        {
            return new SimpleOctreeNode<TData>((SimpleOctreeNode<TData>)parentNode, center, size, level);
        }  
    }
}