using FrameworkSDK.MonoGame.Basic;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Physics2D
{
    public interface IForce2D : IUpdatable
    {
        Vector2 Power { get; }
        float AngularPower { get; }

        void OnAttached(IPhysicsBody2D body);
        
        void OnDetached(IPhysicsBody2D body);
    }
}