using FrameworkSDK;
using FrameworkSDK.Constructing;
using FrameworkSDK.IoC;
using FrameworkSDK.MonoGame.Constructing;

namespace Gates.ClientCore
{
    public static class GameFactory
    {
        public static void Create(IServicesModule services)
        {
            var factory = new AppFactory();
            factory.CreateGame<GameHost>()
                .RegisterServices(x => RegisterServices(x, services))
                .Configure()
                .Run();
        }

        private static void RegisterServices(IServiceRegistrator registrator, IServicesModule services)
        {
            registrator.RegisterModule(new GameCoreModule());
            registrator.RegisterModule(services);
        }
    }
}
