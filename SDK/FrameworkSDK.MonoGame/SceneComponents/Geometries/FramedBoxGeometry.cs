using System;
using FrameworkSDK.MonoGame.Graphics.Basic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.SceneComponents.Geometries
{
    public class FramedBoxGeometry : IMeshGeometry
    {
        public PrimitiveType PrimitiveType { get; } = PrimitiveType.LineList;
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
            return 12;
        }

        private readonly Array _vertices;
        private readonly int[] _indices;
        
        public FramedBoxGeometry(Color color)
        {
            _vertices = new[]
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
            _indices = new[] {0, 1, 0, 2, 0, 3, 1, 6, 1, 7, 2, 7, 2, 5, 3, 5, 3, 6, 4, 5, 4, 6, 4, 7};
        }
    }
}