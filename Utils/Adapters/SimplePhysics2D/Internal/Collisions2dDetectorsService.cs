using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Physics2D;
using JetBrains.Annotations;
using SimplePhysics2D.Detectors;
using SimplePhysics2D.Fixtures;

namespace SimplePhysics2D.Internal
{
    internal class Collisions2dDetectorsService : ICollisions2dDetectorsService 
    {
        private readonly Dictionary<int, ICollisionDetector2D> _collisionDetectors = new Dictionary<int, ICollisionDetector2D>();

        public Collisions2dDetectorsService()
        {
            this.Register<CircleFixture, CircleFixture>(new Circle2CircleCollisionDetector());
        }

        public ICollisionDetector2D GetCollisionDetector(Type fixtureTypeA, Type fixtureTypeB)
        {
            var key = GetKey(fixtureTypeA, fixtureTypeB);
            if (_collisionDetectors.TryGetValue(key, out var detector))
                return detector;
            
            throw new Physics2DException($"No collision detector registered for types {fixtureTypeA}, {fixtureTypeB}");
        }

        public void Register(Type fixtureTypeA, Type fixtureTypeB, [NotNull] ICollisionDetector2D collisionDetector2D)
        {
            if (collisionDetector2D == null) throw new ArgumentNullException(nameof(collisionDetector2D));
            var key = GetKey(fixtureTypeA, fixtureTypeB);
            var reversedKey = GetKey(fixtureTypeB, fixtureTypeA);
            _collisionDetectors.Add(key, collisionDetector2D);
            if (reversedKey != key)
                _collisionDetectors.Add(reversedKey, collisionDetector2D);
        }

        public Collision2D GetCollision(IFixture2D fixtureA, IFixture2D fixtureB)
        {
            return this.GetCollisionDetector(fixtureA, fixtureB)
                .GetCollision(fixtureA, fixtureB);
        }

        private static int GetKey(Type typeA, Type typeB)
        {
            unchecked
            {
                int hash1 = typeA.GetHashCode();
                int hash2 = typeB.GetHashCode();
                return (hash1 * 397) ^ hash2;
            }
        }
    }

    public static class Collisions2dDetectorsServiceExtensions
    {
        public static void Register<T1, T2>(this ICollisions2dDetectorsService collisions2dDetectorsService, ICollisionDetector2D collisionDetector2D)
            where T1 : IFixture2D where T2 : IFixture2D
        {
            collisions2dDetectorsService.Register(typeof(T1), typeof(T2), collisionDetector2D);
        }
    } 
}