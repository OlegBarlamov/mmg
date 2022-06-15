using System;
using FrameworkSDK.MonoGame.Graphics.Basic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.SceneComponents.Geometries
{
    public class RectangleGeometry : IMeshGeometry
    {
        public PrimitiveType PrimitiveType { get; } = PrimitiveType.TriangleList;
        public VertexDeclaration VertexDeclaration { get; } = VertexPositionColor.VertexDeclaration;
        public object GetVertices()
        {
            return _vertices;
        }

        public int[] GetIndices()
        {
            return _indices;
        }

        public int GetPrimitivesCount()
        {
            return 2;
        }
        
        private readonly Array _vertices;
        private readonly int[] _indices;

        public RectangleGeometry(Color color)
        {
            _vertices = new[]
            {
                new VertexPositionColor(new Vector3(-0.5f, 0, -0.5f), color),
                new VertexPositionColor(new Vector3(0.5f, 0, -0.5f), color),
                new VertexPositionColor(new Vector3(0.5f, 0, 0.5f), color),
                new VertexPositionColor(new Vector3(-0.5f, 0, 0.5f), color),
            };

            _indices = new[] {0, 1, 2, 0, 2, 3};
        }
    }
}