using FrameworkSDK.MonoGame.Physics2D;
using SimplePhysics2D;

namespace SimplePhysics2D
{
    public interface ICollisions2DResolver
    {
        void Resolve(Collision2D collision);
    }
}