using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public interface IVideoBuffersFactoryService
    {
        VertexBuffer CreateVertexBugger(VertexDeclaration vertexDeclaration, int vertexCount);

        IndexBuffer CreateIndexBuffer(int indicesCount);
    }
}