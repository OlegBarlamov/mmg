using FrameworkSDK.MonoGame.Graphics.Meshes;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.RenderingTools
{
    public interface IGeometryRenderer
    {
        void SetBuffers(VertexBuffer vertexBuffer, IndexBuffer indexBuffer);
        void Charge(IMeshGeometry geometry);

        void Render(int baseVertex = 0, int startIndex = 0);
    }
}