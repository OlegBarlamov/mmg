using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGameExtensions;

namespace FrameworkSDK.MonoGame.Physics2D
{
    public interface IPhysicsBody2D : ILocatable2D, IRotatable
    {
        IPhysicsBody2DParameters Parameters { get; }
        
        IScene2DPhysics Scene { get; set; }
        
        Vector2 Velocity { get; set; }
        float AngularVelocity { get; set; }

        ICollection<IForce2D> ActiveForces { get; }
    }
}