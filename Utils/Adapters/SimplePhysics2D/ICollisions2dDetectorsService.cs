using System;

namespace SimplePhysics2D
{
    public interface ICollisions2dDetectorsService : ICollisions2dDetectorsProvider, ICollisionDetector2D
    {
        void Register(Type fixtureTypeA, Type fixtureTypeB, ICollisionDetector2D collisionDetector2D);
    }
}