using FrameworkSDK.MonoGame.Physics2D;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using NetExtensions.Helpers;

namespace SimplePhysics2D.Fixtures
{
    public class CircleFixture : Fixture2D
    {
        public float Radius { get; set; }

        public CircleFixture([NotNull] IColliderBody2D parentBody, Vector2 center, float radius)
            : base(parentBody, center)
        {
            Radius = radius;
        }

        public override bool Contains(Vector2 point)
        {
            var squaredDistance = MathExtended.Sqr(Center.X - point.X) + MathExtended.Sqr(Center.Y - point.Y);
            var squaredRadius = MathExtended.Sqr(Radius);
            return squaredDistance < squaredRadius;
        }
    }
}