using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGameExtensions.DataStructures
{
    public interface IOctreeNode<TData>
    {
        event Action<IOctreeNode<TData>> NodeSubdivided; 
        Vector3 Center { get; }
        float Size { get; }
        int Level { get; }
        IReadOnlyCollection<TData> DataObjects { get; }
        BoundingBox BoundingBox { get; }
        OctreeChildrenNodes<TData> Children { get; }
        IOctreeNode<TData> Parent { get; }
    }
}