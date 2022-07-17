using System;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.SceneComponents.Geometries
{
    public class CubeGeometry : StaticMeshGeometry<VertexPositionNormalTexture>
    {
        public CubeGeometry() : base(VertexPositionColorTexture.VertexDeclaration, PrimitiveType.TriangleList, CreateVertices(), CreateIndices(), 12)
        {
        }

        /// <summary>
        ///   7    6
        ///   *----*
        ///  /    / 
        /// *----*
        /// 4    5
        /// 
        ///   3     2
        ///   *----*
        ///  /    / 
        /// *----*
        /// 0    1
        /// </summary>
        private static VertexPositionNormalTexture[] CreateVertices()
        {
            return new[]
            {
                new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), Vector3.Up, new Vector2(0, 0)), 
                new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), Vector3.Up, new Vector2(1, 0)),
                new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), Vector3.Up, new Vector2(1, 1)),
                new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), Vector3.Up, new Vector2(0, 1)),
                
                new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, -0.5f), Vector3.Up, new Vector2(0, 0)),
                new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.5f), Vector3.Up, new Vector2(1, 0)),
                new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0.5f), Vector3.Up, new Vector2(1, 1)),
                new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), Vector3.Up, new Vector2(0, 1)),
            };
        }

        private static short[] CreateIndices()
        {
            return new short[]
            {
                2, 1, 0, 0, 3, 2,
                6, 5, 1, 1, 2, 6,
                5, 4, 0, 0, 1, 5,
                4, 7, 3, 3, 0, 4,
                3, 7, 6, 6, 2, 3,
                4, 5, 6, 6, 7, 4,
            };
        }
    }
}