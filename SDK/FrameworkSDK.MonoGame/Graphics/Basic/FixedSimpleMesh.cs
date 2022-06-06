using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.Basic
{
    public sealed class FixedSimpleMesh : IRenderableMesh
    {
        public IRenderableComponent Parent { get; }
        
        public IMeshGeometry Geometry { get; }
        
        public Matrix World { get; set; } = Matrix.Identity;

        private Vector3 _position = Vector3.Zero;
        private Vector3 _scale = Vector3.One;

        public FixedSimpleMesh([NotNull] IRenderableComponent parent, PrimitiveType primitiveType, VertexDeclaration vertexDeclaration,
            [NotNull] Array vertices, [NotNull] int[] indices, int primitivesCount)
        {
            if (vertices == null) throw new ArgumentNullException(nameof(vertices));
            if (indices == null) throw new ArgumentNullException(nameof(indices));
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
            Geometry = new BasicMeshGeometry(primitiveType, vertexDeclaration, vertices, indices, primitivesCount);
        }

        public FixedSimpleMesh([NotNull] IRenderableComponent parent, [NotNull] IMeshGeometry geometry)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
            Geometry = geometry ?? throw new ArgumentNullException(nameof(geometry));
        }

        public void SetPosition(Vector3 position)
        {
            _position = position;
            UpdateWordMatrix();
        }

        public void SetScale(Vector3 scale)
        {
            _scale = scale;
            UpdateWordMatrix();
        }

        public void UpdateWordMatrix()
        {
            World = Matrix.Identity * Matrix.CreateScale(_scale) * Matrix.CreateTranslation(_position);
        }
    }
}