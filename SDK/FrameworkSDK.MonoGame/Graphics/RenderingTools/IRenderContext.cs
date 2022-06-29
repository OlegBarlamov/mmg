using System;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using Microsoft.Xna.Framework.Graphics;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Graphics
{
    public interface IRenderContext : IDisposable
    {
        IGeometryRenderer GeometryRenderer { get; }
        
        void DrawIndexedPrimitives(PrimitiveType primitiveType, int baseVertex, int startIndex, int primitiveCount);

        void SetVertexBuffer(VertexBuffer vertexBuffer);

        void SetIndexBuffer(IndexBuffer indexBuffer);

        void FillIndexBuffer(IndexBuffer indexBuffer, Array indicesArray);
    }
}
