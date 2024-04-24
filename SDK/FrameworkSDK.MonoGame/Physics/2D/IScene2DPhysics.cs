using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Physics2D
{
    public interface IScene2DPhysics
    {
        IReadOnlyCollection<IForce2D> GlobalForces { get; }
        void AddBody(IPhysicsBody2D body);
        void RemoveBody(IPhysicsBody2D body);
        
        void SetVelocity(IPhysicsBody2D body, Vector2 velocity);
        void SetAngularVelocity(IPhysicsBody2D body2D, float velocity);

        void ApplyForce(IPhysicsBody2D body, IForce2D force);
        void RemoveForce(IPhysicsBody2D body, IForce2D force);

        void AddGlobalForce(IForce2D force);

        void RemoveGlobalForce(IForce2D force2D);

        void ApplyImpulse(IPhysicsBody2D body, Vector2 impulse);

        void ApplyAngularImpulse(IPhysicsBody2D body, float impulse);
    }
}