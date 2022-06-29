using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.Meshes
{
    public class StaticMeshGeometry<TVertexType> : MeshGeometryBase<TVertexType> where TVertexType : struct, IVertexType
    {
        [NotNull] public TVertexType[] Vertices { get; }
        public int PrimitivesCount { get; }
        [NotNull] public Array IndicesArray { get; }

        public StaticMeshGeometry(VertexDeclaration vertexDeclaration, PrimitiveType primitiveType,
            [NotNull] TVertexType[] vertices, [NotNull] int[] indices, int primitivesCount) : base(vertexDeclaration, primitiveType)
        {
            Vertices = vertices ?? throw new ArgumentNullException(nameof(vertices));
            PrimitivesCount = primitivesCount;
            IndicesArray = IndicesBuffersFactory.CreateIndicesArray(indices);
        }

        public StaticMeshGeometry(VertexDeclaration vertexDeclaration, PrimitiveType primitiveType,
            [NotNull] TVertexType[] vertices, [NotNull] short[] indices, int primitivesCount) : base(vertexDeclaration, primitiveType)
        {
            Vertices = vertices ?? throw new ArgumentNullException(nameof(vertices));
            PrimitivesCount = primitivesCount;
            IndicesArray = IndicesBuffersFactory.CreateShortIndicesArray(indices);
        }
        
        public override int GetPrimitivesCount()
        {
            return PrimitivesCount;
        }

        public override TVertexType[] GetVertices()
        {
            return Vertices;
        }

        public override Array GetIndices()
        {
            return IndicesArray;
        }
    }
}