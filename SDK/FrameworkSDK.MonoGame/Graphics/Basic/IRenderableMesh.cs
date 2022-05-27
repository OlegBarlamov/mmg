using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.Basic
{
    public interface IRenderableMesh
    {
        IRenderableComponent Parent { get; }

        PrimitiveType PrimitiveType { get; }
        VertexDeclaration VertexDeclaration { get; }
        object GetVertices();
        int[] GetIndices();
        int GetPrimitivesCount();
    }
}