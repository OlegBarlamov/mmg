using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGameExtensions
{
    public class OctreeNode<TData>
    {
        public BoundingBox BoundingBox { get; }

        public TData Data { get; }

        public IReadOnlyList<OctreeNode<TData>> Children { get; }
    }
}