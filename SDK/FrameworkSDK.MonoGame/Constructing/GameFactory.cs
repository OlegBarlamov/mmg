namespace FrameworkSDK.MonoGame.Constructing
{
    public static class GameFactory
    {
        public static IGameConfigurator<TGame> Create<TGame>(this AppFactory appFactory) where TGame : IGameHost
        {
            var pipeline = appFactory.CreateDefaultPipeline();
            var configurator = appFactory.Create(pipeline);
            return configurator.UseGameFramework<TGame>();
        }
    }
}
