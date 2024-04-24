using FrameworkSDK.MonoGame.Constructing;
using FrameworkSDK.MonoGame.Physics2D;

namespace SimplePhysics2D.Module
{
    public static class GameConstructorExtensions
    {
        public static IGameFactory UsePhysics(this IGameFactory gameFactory, SimplePhysicsScene2DParameters parameters)
        {
            var module = new SimplePhysicsModule(parameters);
            gameFactory.AddServices(module);
            return gameFactory;
        }
    }
}