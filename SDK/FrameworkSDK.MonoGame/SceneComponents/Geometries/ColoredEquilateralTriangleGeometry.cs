using System;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.SceneComponents.Geometries
{
    public class ColoredEquilateralTriangleGeometry : StaticMeshGeometry<VertexPositionColor>
    {
        public ColoredEquilateralTriangleGeometry(Color color)
            : base(VertexPositionColor.VertexDeclaration, PrimitiveType.TriangleList, CreateVertices(color), CreateIndices(), primitivesCount: 1)
        {
        }
        
        private static VertexPositionColor[] CreateVertices(Color color)
        {
            var sqrt3div3 = 0.577350269189626f;
            return new[]
            {
                new VertexPositionColor(new Vector3(0, 0, sqrt3div3), color),
                new VertexPositionColor(new Vector3(-0.5f, 0, -sqrt3div3/2), color),
                new VertexPositionColor(new Vector3(0.5f, 0, -sqrt3div3/2), color),
            };
        }

        private static short[] CreateIndices()
        {
            return new short[] {0, 1, 2};
        }
    }
}