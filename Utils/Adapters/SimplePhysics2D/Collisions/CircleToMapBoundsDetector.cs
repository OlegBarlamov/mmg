using System;
using Microsoft.Xna.Framework;
using NetExtensions.Helpers;
using SimplePhysics2D.Fixtures;

namespace SimplePhysics2D.Collisions
{
    public class CircleToMapBoundsDetector : BaseCollisionDetector2D<CircleFixture, Map2DBoundsFixture>
    {
        public override Collision2D GetCollision(CircleFixture circle, Map2DBoundsFixture mapBounds)
        {
            if (circle.Center.X - circle.Radius < mapBounds.Rectangle.Left ||
                circle.Center.X + circle.Radius > mapBounds.Rectangle.Right ||
                circle.Center.Y - circle.Radius < mapBounds.Rectangle.Top || 
                circle.Center.Y + circle.Radius > mapBounds.Rectangle.Bottom)
            {
                var closestPoint = new Vector2(
                    circle.Center.X.Clamp(mapBounds.Rectangle.Left + circle.Radius, mapBounds.Rectangle.Right - circle.Radius),
                    circle.Center.Y.Clamp(mapBounds.Rectangle.Top + circle.Radius, mapBounds.Rectangle.Bottom - circle.Radius)
                );
                
                // Calculate penetration vector
                var penetration = closestPoint - circle.Center;
                return new Collision2D(circle, mapBounds, closestPoint, penetration);
            }

            return Collision2D.Empty;
        }
    }
}