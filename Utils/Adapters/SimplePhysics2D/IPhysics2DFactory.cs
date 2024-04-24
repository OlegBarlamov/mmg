using FrameworkSDK.MonoGame.Physics;
using JetBrains.Annotations;

namespace SimplePhysics2D
{
    public interface IPhysics2DFactory
    {
        IScene2DPhysicsInternal Create([NotNull] ICollidersSpace2D collidersSpace2D);
    }
}