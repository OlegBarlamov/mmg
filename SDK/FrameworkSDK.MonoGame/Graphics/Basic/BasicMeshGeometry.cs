using System;
using FrameworkSDK.Common;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.Basic
{
    public class BasicMeshGeometry : IMeshGeometry
    {
        public PrimitiveType PrimitiveType { get; }
        
        public VertexDeclaration VertexDeclaration { get; }

        public object GetVertices()
        {
            return _vertices;
        }

        public int[] GetIndices()
        {
            return _indices;
        }

        public int GetPrimitivesCount()
        {
            return _primitivesCount;
        }

        private readonly object _vertices;
        private readonly int[] _indices;
        private readonly int _primitivesCount;

        public BasicMeshGeometry(PrimitiveType primitiveType, VertexDeclaration vertexDeclaration, [NotNull] object vertices,
            [NotNull] int[] indices, int primitivesCount)
        {
            PrimitiveType = primitiveType;
            VertexDeclaration = vertexDeclaration;
            _vertices = vertices ?? throw new ArgumentNullException(nameof(vertices));
            _indices = indices ?? throw new ArgumentNullException(nameof(indices));
            _primitivesCount = primitivesCount;
        }
    }
}