using System.Linq;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.SceneComponents.Geometries
{
    public class IcosahedronGeometry : StaticMeshGeometry<VertexPositionNormalTexture>
    {
        public IcosahedronGeometry() : base(VertexPositionNormalTexture.VertexDeclaration, PrimitiveType.TriangleList, CreateVertices(), CreateIndices(), 20)
        {
            this.FillNormals();
            this.FillTextureCoordinatesSphere();
        }

        public static Vector3[] CreateVectors()
        {
            var x = 0.525731112119133606f;
            var z = 0.850650808352039932f;

            return new[]
            {
                new Vector3(-x, 0f, z),
                new Vector3(x, 0f, z),
                new Vector3(-x, 0f, -z),
                new Vector3(x, 0f, -z),
                new Vector3(0f, z, x),
                new Vector3(0f, z, -x),
                new Vector3(0f, -z, x),
                new Vector3(0f, -z, -x),
                new Vector3(z, x, 0f),
                new Vector3(-z, x, 0f),
                new Vector3(z, -x, 0f),
                new Vector3(-z, -x, 0f)
            };
        }

        public static short[] CreateIndices()
        {
            return new short[]
            {
                0, 4, 1,
                0, 9, 4,
                9, 5, 4,
                4, 5, 8,
                4, 8, 1,
                8, 10, 1,
                8, 3, 10,
                5, 3, 8,
                5, 2, 3,
                2, 7, 3,
                7, 10, 3,
                7, 6, 10,
                7, 11, 6,
                11, 0, 6,
                0, 1, 6,
                6, 1, 10,
                9, 0, 11,
                9, 11, 2,
                9, 2, 5,
                7, 2, 11
            };
        }

        private static VertexPositionNormalTexture[] CreateVertices()
        {
            return CreateVectors().Select(x => new VertexPositionNormalTexture(x, Vector3.Zero, Vector2.Zero)).ToArray();
        }
    }
}