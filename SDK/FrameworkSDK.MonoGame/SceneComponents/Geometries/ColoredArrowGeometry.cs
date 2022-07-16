using FrameworkSDK.MonoGame.Graphics.Meshes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.SceneComponents.Geometries
{
    public class ColoredArrowGeometry : StaticMeshGeometry<VertexPositionColor>
    {
        public ColoredArrowGeometry(Color color)
            : base(VertexPositionColor.VertexDeclaration, PrimitiveType.LineList, CreateVertices(color), CreateIndices(), primitivesCount: 5)
        {
        }
        
        private static VertexPositionColor[] CreateVertices(Color color)
        {
            return new[]
            {
                new VertexPositionColor(new Vector3(0, 0, 0), color),
                new VertexPositionColor(new Vector3(1, 0, 0), color),
                
                new VertexPositionColor(new Vector3(0.9f, 0, 0.1f), color),
                new VertexPositionColor(new Vector3(0.9f, 0, -0.1f), color),

                new VertexPositionColor(new Vector3(0.9f, 0.1f, 0), color),
                new VertexPositionColor(new Vector3(0.9f, -0.1f, 0), color),
            };
        }

        private static short[] CreateIndices()
        {
            return new short[] {0, 1, 1, 2, 1, 3, 1, 4, 1, 5};
        }
    }
}