using FrameworkSDK.MonoGame.Graphics.Meshes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.SceneComponents.Geometries
{
    public class ColoredFramedBoxWithDiagonalsGeometry : StaticMeshGeometry<VertexPositionColor>
    {
        public ColoredFramedBoxWithDiagonalsGeometry(Color color)
            : base(VertexPositionColor.VertexDeclaration, PrimitiveType.LineList, CreateVertices(color), CreateIndices(), primitivesCount: 18)
        {
        }

        private static VertexPositionColor[] CreateVertices(Color color)
        {
            return new[]
            {
                new VertexPositionColor(new Vector3(-0.5f, -0.5f, -0.5f), color), // 0
                new VertexPositionColor(new Vector3( 0.5f, -0.5f, -0.5f), color), // 1
                new VertexPositionColor(new Vector3(-0.5f,  0.5f, -0.5f), color), // 2
                new VertexPositionColor(new Vector3(-0.5f, -0.5f,  0.5f), color), // 3

                new VertexPositionColor(new Vector3( 0.5f,  0.5f,  0.5f), color), // 4
                new VertexPositionColor(new Vector3(-0.5f,  0.5f,  0.5f), color), // 5
                new VertexPositionColor(new Vector3( 0.5f, -0.5f,  0.5f), color), // 6
                new VertexPositionColor(new Vector3( 0.5f,  0.5f, -0.5f), color), // 7
            };
        }

        private static short[] CreateIndices()
        {
            return new short[]
            {
                // 12 box edges
                0, 1, 0, 2, 0, 3,
                1, 6, 1, 7, 2, 7,
                2, 5, 3, 5, 3, 6,
                4, 5, 4, 6, 4, 7,
                // 6 face diagonals (one per face)
                0, 4, // front-back body diagonal through center
                1, 5, // right-to-left diagonal
                2, 6, // top-left-back to bottom-right-front
                0, 7, // bottom face: (-,-,-) to (+,+,-)
                3, 4, // back face:  (-,-,+) to (+,+,+)
                1, 3, // bottom face: (+,-,-) to (-,-,+)
            };
        }
    }
}
