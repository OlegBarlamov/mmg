using System.Linq;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.SceneComponents.Geometries
{
    public class TetrahedronGeometry : StaticMeshGeometry<VertexPositionNormalTexture>
    {
        public TetrahedronGeometry() : base(VertexPositionNormalTexture.VertexDeclaration, PrimitiveType.TriangleList, CreateVertices(), CreateIndices(), 4)
        {
            this.FillNormals();
            this.FillTextureCoordinatesSphere();
        }

        public static Vector3[] CreateVectors()
        {
            var sqrt3div3 = 0.577350269189626f;

            return new[]
            {
                new Vector3(0, 0, sqrt3div3),
                new Vector3(-0.5f, 0, -sqrt3div3/2),
                new Vector3(0.5f, 0, -sqrt3div3/2),
                new Vector3(0, sqrt3div3, 0),
            };
        }

        public static short[] CreateIndices()
        {
            return new short[]
            {
                2, 1, 0,
                3, 2, 0,
                1, 3, 0,
                3, 1, 2
            };
        }

        private static VertexPositionNormalTexture[] CreateVertices()
        {
            return CreateVectors().Select(x => new VertexPositionNormalTexture(x, Vector3.Zero, Vector2.Zero)).ToArray();
        }
    }
}