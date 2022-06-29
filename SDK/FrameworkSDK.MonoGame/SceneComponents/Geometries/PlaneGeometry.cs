using System;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.SceneComponents.Geometries
{
    public class PlaneGeometry : StaticMeshGeometry<VertexPositionNormalTexture>
    {
        public PlaneGeometry() : base(VertexPositionColorTexture.VertexDeclaration, PrimitiveType.TriangleList, CreateVertices(), new short[] {0, 1, 2, 0, 2, 3}, 2)
        {
        }

        private static VertexPositionNormalTexture[] CreateVertices()
        {
            return new[]
            {
                new VertexPositionNormalTexture(new Vector3(-0.5f, 0, -0.5f), Vector3.Up, new Vector2(0, 0)),
                new VertexPositionNormalTexture(new Vector3(0.5f, 0, -0.5f), Vector3.Up, new Vector2(1, 0)),
                new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0.5f), Vector3.Up, new Vector2(1, 1)),
                new VertexPositionNormalTexture(new Vector3(-0.5f, 0, 0.5f), Vector3.Up, new Vector2(0, 1)),
            };
        }
    }
}