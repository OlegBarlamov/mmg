using FrameworkSDK.MonoGame.Physics2D;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions.Geometry;

namespace SimplePhysics2D.Fixtures
{
    public class Map2DBoundsFixture : Fixture2D
    {
        public RectangleF Rectangle { get; set; }

        public Map2DBoundsFixture([NotNull] IColliderBody2D parentBody, RectangleF rectangle)
            : base(parentBody, rectangle.Center)
        {
            Rectangle = rectangle;
        }

        public override bool Contains(Vector2 point)
        {
            return Rectangle.Contains(point);
        }
    }
}