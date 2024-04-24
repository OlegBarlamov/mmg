using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Physics;
using FrameworkSDK.MonoGame.Physics2D;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions;
using NetExtensions.Collections;
using NetExtensions.Helpers;

namespace SimplePhysics2D.Internal
{
    internal class SimpleScene2DPhysics : IScene2DPhysicsInternal
    {
        public IReadOnlyCollection<IForce2D> GlobalForces => _globalForces;
        public SimplePhysicsScene2DParameters Parameters { get; }
        
        private ICollidersSpace2D CollidersSpace { get; }
        private ICollisions2DResolver CollisionsResolver { get; }
        private ICollisionDetector2D CollisionDetector { get; }

        private const float SimulationSpeedFactor = 0.01f;
        
        private readonly IAutoSizeArray<Collision2D> _collisionsArray = new AutoSizeChunkedArray<Collision2D>(1000);

        private readonly ManualUpdatableCollection<IPhysicsBody2D> _simpleBodies = new ManualUpdatableCollection<IPhysicsBody2D>();
        private readonly ManualUpdatableCollection<IColliderBody2D> _colliders = new ManualUpdatableCollection<IColliderBody2D>();
        private readonly ManualUpdatableCollection<IForce2D> _globalForces = new ManualUpdatableCollection<IForce2D>();

        public SimpleScene2DPhysics(
            [NotNull] SimplePhysicsScene2DParameters parameters,
            [NotNull] ICollidersSpace2D collidersSpace,
            [NotNull] ICollisions2DResolver collisionsResolver,
            [NotNull] ICollisionDetector2D collisionDetector
            )
        {
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            CollidersSpace = collidersSpace ?? throw new ArgumentNullException(nameof(collidersSpace));
            CollisionsResolver = collisionsResolver ?? throw new ArgumentNullException(nameof(collisionsResolver));
            CollisionDetector = collisionDetector ?? throw new ArgumentNullException(nameof(collisionDetector));
        }

        public void AddBody([NotNull] IPhysicsBody2D body)
        {
            if (body == null) throw new ArgumentNullException(nameof(body));
            if (body is IColliderBody2D colliderBody)
            {
                _colliders.Add(colliderBody);
                CollidersSpace.AddBody(colliderBody);
            }
            else
            {
                _simpleBodies.Add(body);
            }

            body.Scene = this;
        }

        public void RemoveBody([NotNull] IPhysicsBody2D body)
        {
            if (body == null) throw new ArgumentNullException(nameof(body));
            if (body is IColliderBody2D colliderBody)
            {
                _colliders.Remove(colliderBody);
                CollidersSpace.RemoveBody(colliderBody);
            }
            else
            {
                _simpleBodies.Remove(body);
            }
            
            body.Scene = null;
        }

        public void SetVelocity([NotNull] IPhysicsBody2D body, Vector2 velocity)
        {
            if (body == null) throw new ArgumentNullException(nameof(body));
            body.Velocity = velocity;
        }

        public void SetAngularVelocity([NotNull] IPhysicsBody2D body, float velocity)
        {
            if (body == null) throw new ArgumentNullException(nameof(body));
            body.AngularVelocity = velocity;
        }

        public void ApplyForce(IPhysicsBody2D body, [NotNull] IForce2D force)
        {
            if (force == null) throw new ArgumentNullException(nameof(force));
            body.ActiveForces.Add(force);
            force.OnAttached(body);
        }

        public void RemoveForce(IPhysicsBody2D body, [NotNull] IForce2D force)
        {
            if (force == null) throw new ArgumentNullException(nameof(force));
            body.ActiveForces.Remove(force);
            force.OnDetached(body);
        }

        public void AddGlobalForce([NotNull] IForce2D force)
        {
            if (force == null) throw new ArgumentNullException(nameof(force));
            _globalForces.Add(force);
        }

        public void RemoveGlobalForce(IForce2D force)
        {
            if (force == null) throw new ArgumentNullException(nameof(force));
            _globalForces.Remove(force);
        }

        public void ApplyImpulse([NotNull] IPhysicsBody2D body, Vector2 impulse)
        {
            if (body == null) throw new ArgumentNullException(nameof(body));
            body.Velocity += impulse / body.Parameters.Mass;
        }

        public void ApplyAngularImpulse(IPhysicsBody2D body, float impulse)
        {
            if (body == null) throw new ArgumentNullException(nameof(body));
            body.AngularVelocity += impulse / body.Parameters.Mass;
        }

        public void Update(GameTime gameTime)
        {
            _simpleBodies.UpdateStarted();
            _colliders.UpdateStarted();
            _globalForces.UpdateStarted();
            
            _collisionsArray.ResetIndex();
            try
            {
                foreach (var body in _colliders)
                {
                    UpdateBody(body, gameTime, out var positionChanged);
                    if (positionChanged)
                    {
                        var candidates = CollidersSpace.GetCollisionCandidates(body);
                        foreach (var candidate in candidates)
                        {
                            var collision = CollisionDetector.GetCollision(body.Fixture, candidate.Fixture);
                            if (!collision.IsEmpty)
                            {
                                _collisionsArray.Add(collision);
                            }
                        }
                    }
                }

                foreach (var body in _simpleBodies)
                {
                    UpdateBody(body, gameTime, out _);
                }

                foreach (var collision in _collisionsArray)
                {
                    CollisionsResolver.Resolve(collision);
                }
            }
            finally
            {
                _simpleBodies.UpdateFinished(true);
                _colliders.UpdateFinished(true);
                _globalForces.UpdateFinished(true);
            }
        }

        private void UpdateBody(IPhysicsBody2D body, GameTime gameTime, out bool positionChanged)
        {
            positionChanged = false;
            if (body.Parameters.ForcesTarget)
            {
                foreach (var force in GlobalForces)
                {
                    force.Update(gameTime);
                    ApplyForce(body, force, gameTime);
                }
                foreach (var force in body.ActiveForces)
                {
                    force.Update(gameTime);
                    ApplyForce(body, force, gameTime);
                }
            }

            body.SetRotation(body.Rotation + body.AngularVelocity * GetTimeIntervalFactor(gameTime));
            body.AngularVelocity = MathExtended.DecreaseByModule(body.AngularVelocity, body.Parameters.AngularFrictionFactor / body.Parameters.Mass * GetTimeIntervalFactor(gameTime));

            if (body.Velocity != Vector2.Zero)
            {
                positionChanged = true;
                body.SetPosition(body.Position + body.Velocity * GetTimeIntervalFactor(gameTime));
                body.Velocity = body.Velocity.DecreaseMagnitude(body.Parameters.FrictionFactor / body.Parameters.Mass * GetTimeIntervalFactor(gameTime));
            }
        }
        
        private void ApplyForce(IPhysicsBody2D body, IForce2D force, GameTime gameTime)
        {
            body.Velocity += force.Power / body.Parameters.Mass * GetTimeIntervalFactor(gameTime);
            body.AngularVelocity += force.AngularPower / body.Parameters.Mass * GetTimeIntervalFactor(gameTime);
        }
        
        private float GetTimeIntervalFactor(GameTime gameTime)
        {
            return gameTime.ElapsedGameTime.Milliseconds * Parameters.SimulationSpeed * SimulationSpeedFactor;
        }
    }
}