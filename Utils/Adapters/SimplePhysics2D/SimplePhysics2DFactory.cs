using System;
using FrameworkSDK.MonoGame.Physics;
using FrameworkSDK.MonoGame.Physics2D;
using JetBrains.Annotations;
using SimplePhysics2D.Internal;

namespace SimplePhysics2D
{
    class SimplePhysics2DFactory : IPhysics2DFactory
    {
        public SimplePhysicsScene2DParameters Parameters { get; }
        public ICollisions2DResolver Collisions2DResolver { get; }
        public ICollisions2dDetectorsService Collisions2dDetectorsService { get; }

        public SimplePhysics2DFactory(
            [NotNull] SimplePhysicsScene2DParameters parameters,
            [NotNull] ICollisions2DResolver collisions2DResolver,
            [NotNull] ICollisions2dDetectorsService collisions2dDetectorsService)
        {
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            Collisions2DResolver = collisions2DResolver ?? throw new ArgumentNullException(nameof(collisions2DResolver));
            Collisions2dDetectorsService = collisions2dDetectorsService ?? throw new ArgumentNullException(nameof(collisions2dDetectorsService));
        }
        
        public IScene2DPhysicsInternal Create(ICollidersSpace2D collidersSpace2D)
        {
            if (collidersSpace2D == null) throw new ArgumentNullException(nameof(collidersSpace2D));
            return new SimpleScene2DPhysics(Parameters, collidersSpace2D, Collisions2DResolver, Collisions2dDetectorsService);
        }
    }
}