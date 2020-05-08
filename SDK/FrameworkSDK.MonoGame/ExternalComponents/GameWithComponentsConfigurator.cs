using System;
using FrameworkSDK.Constructing;
using FrameworkSDK.MonoGame.Constructing;
using FrameworkSDK.Pipelines;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.ExternalComponents
{
    public interface IGameWithComponentsConfigurator<TGame> : IGameConfigurator<TGame> where TGame : GameApp
    {
        
    }

    internal class GameWithComponentsConfigurator<TGame> : IGameWithComponentsConfigurator<TGame> where TGame : GameApp
    {
        public Pipeline ConfigurationPipeline => GameConfigurator.ConfigurationPipeline;
        
        public IExternalGameComponentsService GameComponentsService { get; }
        
        private IGameConfigurator<TGame> GameConfigurator { get; }

        public GameWithComponentsConfigurator([NotNull] IGameConfigurator<TGame> gameConfigurator)
        {
            GameConfigurator = gameConfigurator ?? throw new ArgumentNullException(nameof(gameConfigurator));
            GameComponentsService = new ExternalGameComponentsService();
        }
        
        public void Dispose()
        {
            GameConfigurator.Dispose();
        }
        
        public IAppRunner Configure()
        {
            return GameConfigurator.Configure();
        }
    }
}