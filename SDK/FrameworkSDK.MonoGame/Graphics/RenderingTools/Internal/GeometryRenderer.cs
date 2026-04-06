using System;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.RenderingTools
{
    internal class GeometryRenderer : IGeometryRenderer
    {
        private IRenderContext RenderContext { get; }

        private int _chargedPrimitiveCount;
        private PrimitiveType _chargedPrimitiveType;
        
        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;

        public GeometryRenderer([NotNull] IRenderContext renderContext)
        {
            RenderContext = renderContext ?? throw new ArgumentNullException(nameof(renderContext));
        }

        public void SetBuffers(VertexBuffer vertexBuffer, IndexBuffer indexBuffer)
        {
            _vertexBuffer = vertexBuffer;
            _indexBuffer = indexBuffer;

            RenderContext.SetVertexBuffer(_vertexBuffer);
            RenderContext.SetIndexBuffer(_indexBuffer);
        }

        public void Charge(IMeshGeometry geometry)
        {
            _chargedPrimitiveCount = geometry.GetPrimitivesCount();
            _chargedPrimitiveType = geometry.PrimitiveType;

            var vertices = geometry.GetVertices();
            var indices = geometry.GetIndices();
            var geometryName = geometry.GetType().Name;

            if (vertices.Length > _vertexBuffer.VertexCount)
                throw new InvalidOperationException(
                    $"Vertex buffer overflow: geometry '{geometryName}' has {vertices.Length} vertices, but buffer capacity is {_vertexBuffer.VertexCount}.");

            var indexCount = indices.Length;
            if (indexCount > _indexBuffer.IndexCount)
                throw new InvalidOperationException(
                    $"Index buffer overflow: geometry '{geometryName}' has {indexCount} indices, but buffer capacity is {_indexBuffer.IndexCount}.");

            geometry.FillVertexBuffer(_vertexBuffer);
            RenderContext.FillIndexBuffer(_indexBuffer, indices);
        }

        public void Render(int baseVertex = 0, int startIndex = 0)
        {
            RenderContext.DrawIndexedPrimitives(_chargedPrimitiveType, baseVertex, startIndex, _chargedPrimitiveCount);
        }
    }
}