using FrameworkSDK.MonoGame.Physics2D;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Physics._2D.Forces
{
    public class SimpleForce : IForce2D
    {
        public Vector2 Power { get; set; }
        public float AngularPower { get; set; }
        
        public SimpleForce(Vector2 power, float angularPower)
        {
            Power = power;
            AngularPower = angularPower;
        }
        
        public void Update(GameTime gameTime)
        {
            
        }

        public void OnAttached(IPhysicsBody2D body)
        {
        }

        public void OnDetached(IPhysicsBody2D body)
        {
        }
    }
}