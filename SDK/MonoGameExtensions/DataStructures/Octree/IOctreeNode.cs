using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGameExtensions.DataStructures
{
    public interface IOctreeNode<TData>
    {
        Vector3 Center { get; }
        float Size { get; }
        int Level { get; }
        TData Data { get; }
        BoundingBox BoundingBox { get; }
        OctreeChildrenNodes<TData> Children { get; }
    }
}