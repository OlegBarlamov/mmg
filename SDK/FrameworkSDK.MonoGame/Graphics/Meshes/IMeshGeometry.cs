using System;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.Meshes
{
    public interface IMeshGeometry
    {
        PrimitiveType PrimitiveType { get; }
        VertexDeclaration VertexDeclaration { get; }
        Array GetVertices();
        Array GetIndices();
        int GetPrimitivesCount();

        void FillVertexBuffer(VertexBuffer vertexBuffer);
    }
}