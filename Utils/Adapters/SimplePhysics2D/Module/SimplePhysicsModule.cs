using FrameworkSDK.DependencyInjection;
using FrameworkSDK.MonoGame.Physics2D;
using SimplePhysics2D.Internal;

namespace SimplePhysics2D.Module
{
    internal class SimplePhysicsModule : IServicesModule
    {
        public SimplePhysicsScene2DParameters Parameters { get; }

        public SimplePhysicsModule(SimplePhysicsScene2DParameters parameters)
        {
            Parameters = parameters;
        }
        public void RegisterServices(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.RegisterType<ICollisions2DResolver, Elastic2DCollisionsResolver>();
            serviceRegistrator.RegisterType<ICollisions2dDetectorsService, Collisions2dDetectorsService>();
            serviceRegistrator.RegisterFactory<ICollisions2dDetectorsProvider>((locator, type) => locator.Resolve<ICollisions2dDetectorsService>());
            serviceRegistrator.RegisterFactory<ICollisionDetector2D>((locator, type) => locator.Resolve<ICollisions2dDetectorsService>());
            serviceRegistrator.RegisterInstance(Parameters);
            serviceRegistrator.RegisterType<IPhysics2DFactory, SimplePhysics2DFactory>();
        }
    }
}