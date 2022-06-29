using System;
using FrameworkSDK.MonoGame.Graphics.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.RenderingTools
{
    internal class RenderContext : IRenderContext
    {
        public IGeometryRenderer GeometryRenderer { get; }
        private GraphicsDevice GraphicsDevice { get; }
        private IIndicesBuffersFiller IndicesBuffersFiller { get; }

        public RenderContext([NotNull] GraphicsDevice graphicsDevice, [NotNull] IIndicesBuffersFiller indicesBuffersFiller)
        {
            GraphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
            IndicesBuffersFiller = indicesBuffersFiller ?? throw new ArgumentNullException(nameof(indicesBuffersFiller));
            GeometryRenderer = new GeometryRenderer(this);
        }

        public void DrawIndexedPrimitives(
            PrimitiveType primitiveType,
            int baseVertex,
            int startIndex,
            int primitiveCount)
        {
            GraphicsDevice.DrawIndexedPrimitives(primitiveType, baseVertex, startIndex, primitiveCount);
        }

        public void SetVertexBuffer(VertexBuffer vertexBuffer)
        {
            GraphicsDevice.SetVertexBuffer(vertexBuffer);
        }

        public void SetIndexBuffer(IndexBuffer indexBuffer)
        {
            GraphicsDevice.Indices = indexBuffer;
        }

        public void FillIndexBuffer(IndexBuffer indexBuffer, Array indicesArray)
        {
            IndicesBuffersFiller.FillIndexBuffer(indexBuffer, indicesArray);
        }

        public void Dispose()
        {
            
        }
    }
}
