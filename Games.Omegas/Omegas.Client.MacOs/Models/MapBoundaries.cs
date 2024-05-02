using FrameworkSDK.MonoGame.Physics2D;
using Microsoft.Xna.Framework;
using MonoGameExtensions.Geometry;
using NetExtensions.Helpers;
using Omegas.Client.MacOs.Models.SphereObject;
using SimplePhysics2D.Objects;

namespace Omegas.Client.MacOs.Models
{
    public class MapBoundaries : PhysicsMapBounds2D
    {
        public MapBoundaries(RectangleF rectangle)
            : base(rectangle)
        {
        }

        public override bool OnCollision(IColliderBody2D body)
        {
            if (body is SphereObjectData sphere && sphere.Velocity.Length() > 1f)
            {
                var closestPoint = new Vector2(
                    sphere.Position.X.Clamp(Fixture.Rectangle.Left + sphere.Size,
                        Fixture.Rectangle.Right - sphere.Size),
                    sphere.Position.Y.Clamp(Fixture.Rectangle.Top + sphere.Size,
                        Fixture.Rectangle.Bottom - sphere.Size)
                );
                
                // Calculate the collision normal (direction from sphere center to closest point)
                var collisionNormal = closestPoint - sphere.Position;
                collisionNormal.Normalize();
                
                // Reflect the velocity of the sphere based on the collision normal
                var dotProduct = Vector2.Dot(sphere.Velocity, collisionNormal);
                var reflectedVelocity = sphere.Velocity - 2 * dotProduct * collisionNormal;
                
                // Assign the reflected velocity to the sphere
                sphere.Velocity = reflectedVelocity * 0.75f;

                return true;
            }

            return base.OnCollision(body);
        }
    }
}