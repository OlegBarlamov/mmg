using FrameworkSDK.MonoGame.Physics2D;

namespace SimplePhysics2D.Collisions
{
    public abstract class BaseCollisionDetector2D<T1, T2> : ICollisionDetector2D<T1, T2> where T1 : class, IFixture2D where T2 : class, IFixture2D
    {
        public abstract Collision2D GetCollision(T1 bodyA, T2 bodyB);

        public Collision2D GetCollision(IFixture2D bodyA, IFixture2D bodyB)
        {
            return GetCollision((T1) bodyA, (T2) bodyB);
        }
        
        
    }
}