using System;
using FrameworkSDK.Pipelines;
using FrameworkSDK.Game;
using FrameworkSDK.IoC;
using FrameworkSDK.Localization;
using JetBrains.Annotations;

namespace FrameworkSDK.Constructing
{
    internal class GameConfigurator<TGameHost> : IGameConfigurator<TGameHost> where TGameHost : IGameHost
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

        public void Configure()
        {
            AppConfigurator.Configure();
        }

        public void Run()
        {
            AppConfigurator.Run();

            var locator = AppContext.ServiceLocator;
            var host = locator.Resolve<TGameHost>();
            var game = locator.Resolve<IGame>();

            host.Run(game);
        }
    }
}
