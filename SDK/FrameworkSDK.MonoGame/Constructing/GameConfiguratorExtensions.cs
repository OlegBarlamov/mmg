using System;
using FrameworkSDK.Constructing;
using FrameworkSDK.IoC;
using FrameworkSDK.Localization;
using FrameworkSDK.Logging;
using FrameworkSDK.MonoGame.Config;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline.Processing;
using FrameworkSDK.MonoGame.Localization;
using FrameworkSDK.Pipelines;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Constructing
{
	public static class GameConfiguratorExtensions
	{
		internal static IGameConfigurator<THost> UseGameFramework<THost>([NotNull] this IAppConfigurator configurator) where THost : GameApp
		{
			var baseSetupsPhase = configurator.GetStep(GameDefaultConfigurationSteps.BaseSetup);
			baseSetupsPhase.AddAction(new SimplePipelineAction(GameDefaultConfigurationSteps.BaseSetupActions.Setup, true,
				context =>
				{
					Strings.Localization = configurator.GetObjectFromContext<ILocalization>(context, GameDefaultConfigurationSteps.ContextKeys.Localization);
				}));
			
		    var registerPhase = configurator.GetStep(GameDefaultConfigurationSteps.Registration);
		    registerPhase.AddAction(new SimplePipelineAction(
		        GameDefaultConfigurationSteps.RegistrationActions.Game,
                true,
                context =>
                {
                    var registrator = configurator.GetObjectFromContext<IServiceRegistrator>(context, GameDefaultConfigurationSteps.ContextKeys.Container);
                    var module = new GameModule<THost>();
                    registrator.RegisterModule(module);
                }));

		    var constructPhase = configurator.GetStep(GameDefaultConfigurationSteps.Constructing);
            constructPhase.AddAction(new SimplePipelineAction(
                GameDefaultConfigurationSteps.ConstructingActions.Game,
                true,
                context =>
                {
                    var loggerService = configurator.GetObjectFromContext<IFrameworkLogger>(context, GameDefaultConfigurationSteps.ContextKeys.BaseLogger);
                    var serviceLocator = configurator.GetObjectFromContext<IServiceLocator>(context, GameDefaultConfigurationSteps.ContextKeys.Locator);

                    AppContext.Initialize(loggerService, serviceLocator);

                    var associateService = serviceLocator.Resolve<IGraphicsPipelinePassAssociateService>();
                    associateService.Initialize();
                }));

            return new GameConfigurator<THost>(configurator);
		}

	    public static IGameConfigurator<THost> SetupGameParameters<THost>([NotNull] this IGameConfigurator<THost> configurator,
	        [NotNull] Func<IGameParameters> parametersFactory)
	        where THost : GameApp
	    {
	        if (parametersFactory == null) throw new ArgumentNullException(nameof(parametersFactory));

	        var registerPhase = configurator.GetStep(GameDefaultConfigurationSteps.Registration);
	        registerPhase.AddAction(new SimplePipelineAction(
	            GameDefaultConfigurationSteps.RegistrationActions.GameParameters,
	            true,
	            context =>
	            {
	                var registrator = configurator.GetObjectFromContext<IServiceRegistrator>(context, GameDefaultConfigurationSteps.ContextKeys.Container);
	                var gameParameters = configurator.GetFromFactory(parametersFactory);
	                registrator.RegisterInstance(gameParameters);
	            }));

	        return configurator;
	    }
	}
}
