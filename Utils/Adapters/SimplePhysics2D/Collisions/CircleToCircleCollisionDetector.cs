using Microsoft.Xna.Framework;
using NetExtensions.Helpers;
using SimplePhysics2D.Fixtures;

namespace SimplePhysics2D.Collisions
{
    public class CircleToCircleCollisionDetector : BaseCollisionDetector2D<CircleFixture, CircleFixture>
    {
        public override Collision2D GetCollision(CircleFixture bodyA, CircleFixture bodyB)
        {
            var c1 = bodyA.Center;
            var c2 = bodyB.Center;
            Vector2.DistanceSquared(ref c1, ref c2, out var squaredDistance);
            if (squaredDistance >= MathExtended.Sqr(bodyA.Radius + bodyB.Radius))
                return Collision2D.Empty;

            Vector2 tangent = c1 + (c2 - c1) * bodyA.Radius / (bodyA.Radius + bodyB.Radius);
            Vector2 collisionNormal = c1 - c2;
            float penetrationDepth = bodyA.Radius + bodyB.Radius - collisionNormal.Length();
            Vector2 penetrationVector = Vector2.Normalize(collisionNormal) * penetrationDepth;

            return new Collision2D(bodyA, bodyB, tangent, penetrationVector);

        }
    }
}