using FrameworkSDK.MonoGame.Physics2D;
using SimplePhysics2D;

namespace SimplePhysics2D
{
    public interface ICollisionDetector2D
    {
        Collision2D GetCollision(IFixture2D bodyA, IFixture2D bodyB);
    }
    
    public interface ICollisionDetector2D<in T1, in T2> : ICollisionDetector2D where T1 : IFixture2D where T2 : IFixture2D
    {
        Collision2D GetCollision(T1 bodyA, T2 bodyB);
    }
}