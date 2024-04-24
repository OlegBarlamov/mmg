using System;
using FrameworkSDK.MonoGame.Physics2D;

namespace SimplePhysics2D
{
    public interface ICollisions2dDetectorsProvider
    {
        ICollisionDetector2D GetCollisionDetector(Type fixtureTypeA, Type fixtureTypeB);
    }

    public static class Collisions2dDetectorsProviderExtensions
    {
        public static ICollisionDetector2D GetCollisionDetector(
            this ICollisions2dDetectorsProvider collisionsDetectorsProvider, IFixture2D fixtureA, IFixture2D fixtureB)
        {
            return collisionsDetectorsProvider.GetCollisionDetector(fixtureA.GetType(), fixtureB.GetType());
        }
    }
}