using System;
using FrameworkSDK.MonoGame.Physics2D;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace SimplePhysics2D.Fixtures
{
    public abstract class Fixture2D : IFixture2D
    {
        public IColliderBody2D Parent { get; }
        
        public Vector2 Center { get; set; }

        internal Fixture2D([NotNull] IColliderBody2D parentBody, Vector2 center)
        {
            Parent = parentBody ?? throw new ArgumentNullException(nameof(parentBody));
            Center = center;
        }

        public abstract bool Contains(Vector2 point);
    }
}