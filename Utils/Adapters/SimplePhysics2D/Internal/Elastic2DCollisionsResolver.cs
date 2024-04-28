using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Physics2D;
using Microsoft.Xna.Framework;
using SimplePhysics2D.Collisions;
using SimplePhysics2D.Fixtures;

namespace SimplePhysics2D.Internal
{
    internal class Elastic2DCollisionsResolver : ICollisions2DResolver
    {
        private readonly Dictionary<int, ICollisions2DResolver> _resolvers = new Dictionary<int, ICollisions2DResolver>
        {
            { GetKey(typeof(CircleFixture), typeof(CircleFixture)), new GenericCollisionResolver() },
            { GetKey(typeof(CircleFixture), typeof(Map2DBoundsFixture)), new GenericCollisionResolver() }
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

        private class GenericCollisionResolver : ICollisions2DResolver
        {
            public void Resolve(Collision2D collision)
            {
                var bodyA = collision.FixtureA.Parent;
                var bodyB = collision.FixtureB.Parent;
                var normal = collision.PenetrationVector;

                ResolvePenetrationDepth(bodyA, bodyB, normal);
                normal.Normalize();
                ResolveDynamic(bodyA, bodyB, normal);
            }

            private void ResolvePenetrationDepth(IPhysicsBody2D bodyA, IPhysicsBody2D bodyB, Vector2 penetrationVector)
            {
                if (bodyA.Parameters.Static)
                {
                    bodyB.SetPosition(bodyA.Position - penetrationVector);
                } 
                else if (bodyB.Parameters.Static)
                {
                    bodyA.SetPosition(bodyA.Position + penetrationVector);
                }
                else
                {
                    bodyA.SetPosition(bodyA.Position + penetrationVector * 0.5f);
                    bodyB.SetPosition(bodyB.Position - penetrationVector * 0.5f);
                }
            }

            private void ResolveDynamic(IPhysicsBody2D bodyA, IPhysicsBody2D bodyB, Vector2 collisionNormal)
            {
                // Calculate relative velocity
                var relativeVelocity = bodyB.Velocity - bodyA.Velocity;
                float normalRelativeVelocity = Vector2.Dot(relativeVelocity, collisionNormal);
                
                // Calculate impulse
                var b1 = bodyA.Parameters.BounceFactor;
                var b2 = bodyB.Parameters.BounceFactor;
                var m1 = bodyA.Parameters.Mass;
                var m2 = bodyB.Parameters.Mass;
                var impulseMagnitude = -(1 + b1 * b2) * normalRelativeVelocity / (1 / m1 + 1 / m2);
                
                // Apply impulse to circles
                if (!bodyA.Parameters.Static)
                    bodyA.Scene.ApplyImpulse(bodyA, -impulseMagnitude * collisionNormal);
                if (!bodyB.Parameters.Static)
                    bodyB.Scene.ApplyImpulse(bodyB, impulseMagnitude * collisionNormal);
            }
        }
    }
}