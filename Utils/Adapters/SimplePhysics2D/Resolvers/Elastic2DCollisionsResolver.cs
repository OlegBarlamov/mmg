using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SimplePhysics2D.Fixtures;

namespace SimplePhysics2D.Resolvers
{
    internal class Elastic2DCollisionsResolver : ICollisions2DResolver
    {
        private readonly Dictionary<int, ICollisions2DResolver> _resolvers = new Dictionary<int, ICollisions2DResolver>
        {
            { GetKey(typeof(CircleFixture), typeof(CircleFixture)), new Circle2CircleCollisionResolver()} 
        };
        
        public void Resolve(Collision2D collision)
        {
            var key = GetKey(collision.FixtureA.GetType(), collision.FixtureB.GetType());
            if (_resolvers.TryGetValue(key, out var resolver))
            {
                resolver.Resolve(collision);
            }
        }

        private static int GetKey(Type typeA, Type typeB)
        {
            unchecked
            {
                int hash1 = typeA.GetHashCode();
                int hash2 = typeB.GetHashCode();
                return (hash1 * 397) ^ hash2;
            }
        }
        
        private class Circle2CircleCollisionResolver : ICollisions2DResolver
        {
            public void Resolve(Collision2D collision)
            {
                var fixtureA = (CircleFixture)collision.FixtureA;
                var fixtureB = (CircleFixture)collision.FixtureB;
                    
                var collisionNormal = fixtureA.Center - fixtureB.Center;
                float penetrationDepth = fixtureA.Radius + fixtureB.Radius - collisionNormal.Length();
        
                if (penetrationDepth > 0)
                {
                    // Normalize collision normal
                    collisionNormal.Normalize();

                    var bodyA = fixtureA.Parent;
                    var bodyB = fixtureB.Parent;
                    // Move circles away from each other
                    if (bodyA.Parameters.Static)
                    {
                        bodyB.SetPosition(bodyB.Position - collisionNormal * penetrationDepth * 1);
                    } 
                    else if (bodyB.Parameters.Static)
                    {
                        bodyA.SetPosition(bodyA.Position + collisionNormal * penetrationDepth * 1);
                    }
                    else
                    {
                        bodyA.SetPosition(bodyA.Position + collisionNormal * penetrationDepth * 0.5f);
                        bodyB.SetPosition(bodyB.Position - collisionNormal * penetrationDepth * 0.5f);   
                    }

                    // Calculate relative velocity
                    var relativeVelocity = bodyB.Velocity - bodyA.Velocity;
                    float normalRelativeVelocity = Vector2.Dot(relativeVelocity, collisionNormal);

                    // Calculate impulse
                    var bounceA = bodyA.Parameters.BounceFactor;
                    var bounceB = bodyB.Parameters.BounceFactor;
                    var massA = bodyA.Parameters.Mass;
                    var massB = bodyB.Parameters.Mass;
                    float impulse = -(1 + bounceA * bounceB) * normalRelativeVelocity / (1 / massA + 1 / massB);

                    // Apply impulse to circles
                    if (!bodyA.Parameters.Static)
                        bodyA.Scene.ApplyImpulse(bodyA, -impulse * collisionNormal);
                    if (!bodyB.Parameters.Static)
                        bodyB.Scene.ApplyImpulse(bodyB, impulse * collisionNormal);
                }
            }
        }
    }
}