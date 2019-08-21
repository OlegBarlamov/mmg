using System;
using FrameworkSDK.Pipelines;
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
	        try
	        {
		        AppConfigurator.Configure();
			}
	        catch (Exception e)
	        {
		        throw new AppConstructingException(Strings.Exceptions.Constructing.ConstructingFailed, e, nameof(TGameHost));
	        }
        }

        public void Run()
        {
	        IGameHost gameHost;
	        IGame game;

			try
	        {
		        AppConfigurator.Run();

		        var locator = AppContext.ServiceLocator;
		        gameHost = locator.Resolve<IGameHost>();
		        game = locator.Resolve<IGame>();
	        }
	        catch (Exception e)
	        {
		        throw new AppConstructingException(Strings.Exceptions.Constructing.RunAppFailed, e, nameof(TGameHost));
	        }

	        gameHost.Run(game);
        }
    }
}
