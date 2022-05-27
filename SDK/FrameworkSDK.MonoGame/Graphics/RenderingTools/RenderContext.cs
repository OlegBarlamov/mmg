using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.RenderingTools
{
    public class RenderContext : IRenderContext
    {
        private GraphicsDevice GraphicsDevice { get; }

        public RenderContext([NotNull] GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
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

        public void Dispose()
        {
            
        }
    }
}
