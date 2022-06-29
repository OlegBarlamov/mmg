using FrameworkSDK.MonoGame.Graphics.Meshes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.SceneComponents.Geometries
{
    public class ColoredFramedBoxGeometry : StaticMeshGeometry<VertexPositionColor>
    {
        public ColoredFramedBoxGeometry(Color color)
            : base(VertexPositionColor.VertexDeclaration, PrimitiveType.LineList, CreateVertices(color), CreateIndices(), primitivesCount: 12)
        {
        }

        private static VertexPositionColor[] CreateVertices(Color color)
        {
            return new[]
            {
                new VertexPositionColor(new Vector3(-0.5f, -0.5f, -0.5f), color),
                new VertexPositionColor(new Vector3(0.5f, -0.5f, -0.5f), color),
                new VertexPositionColor(new Vector3(-0.5f, 0.5f, -0.5f), color),
                new VertexPositionColor(new Vector3(-0.5f, -0.5f, 0.5f), color),

                new VertexPositionColor(new Vector3(0.5f, 0.5f, 0.5f), color),
                new VertexPositionColor(new Vector3(-0.5f, 0.5f, 0.5f), color),
                new VertexPositionColor(new Vector3(0.5f, -0.5f, 0.5f), color),
                new VertexPositionColor(new Vector3(0.5f, 0.5f, -0.5f), color),
            };
        }

        private static short[] CreateIndices()
        {
            return new short[] {0, 1, 0, 2, 0, 3, 1, 6, 1, 7, 2, 7, 2, 5, 3, 5, 3, 6, 4, 5, 4, 6, 4, 7};
        }
    }
}