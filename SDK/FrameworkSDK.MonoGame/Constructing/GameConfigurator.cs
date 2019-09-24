using System;
using FrameworkSDK.Constructing;
using FrameworkSDK.Localization;
using FrameworkSDK.Pipelines;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Constructing
{
    public interface IGameConfigurator<TGame> : IAppConfigurator where TGame : IGameHost
    {
    }

    internal class GameConfigurator<TGame> : IGameConfigurator<TGame> where TGame : IGameHost
    {
        public Pipeline ConfigurationPipeline => AppConfigurator.ConfigurationPipeline;

        [NotNull] private IAppConfigurator AppConfigurator { get; }

        public GameConfigurator([NotNull] IAppConfigurator appConfigurator)
        {
            AppConfigurator = appConfigurator ?? throw new ArgumentNullException(nameof(appConfigurator));
        }

        public void Dispose()
        {
            AppConfigurator.Dispose();
        }

        public IAppRunner Configure()
        {
            try
            {
                var defaultRunner = AppConfigurator.Configure();
                return new GameRunner<TGame>(defaultRunner);
            }
            catch (Exception e)
            {
                throw new GameConstructingException(Strings.Exceptions.Constructing.ConstructingFailed, e, typeof(TGame).Name);
            }
        }
    }
}