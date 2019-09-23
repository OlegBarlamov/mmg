using System;
using FrameworkSDK.Game;
using FrameworkSDK.IoC;
using FrameworkSDK.Modules;
using JetBrains.Annotations;

namespace FrameworkSDK.Constructing
{
	public static partial class AppConfiguratorExtensions
	{
		public static IGameConfigurator<THost> UseGameFramework<THost>([NotNull] this IAppConfigurator configurator) where THost : IGameHost
		{
		    var registerPhase = configurator.GetStep(DefaultConfigurationSteps.Registration);
		    registerPhase.AddAction(new PipelineAction(
                DefaultConfigurationSteps.RegistrationActions.Game,
                true,
                context =>
                {
                    var registrator = GetObjectFromContext<IServiceRegistrator>(context, DefaultConfigurationSteps.ContextKeys.Container);
                    var module = new GameModule<THost>();
                    registrator.RegisterModule(module);
                }));

		    var constructPhase = configurator.GetStep(DefaultConfigurationSteps.Constructing);
            constructPhase.AddAction(new PipelineAction(
                DefaultConfigurationSteps.ConstructingActions.Game,
                true,
                context =>
                {
                    var locator = GetObjectFromContext<IServiceLocator>(context, DefaultConfigurationSteps.ContextKeys.Locator);
                    var game = locator.Resolve<IGame>();
                    var host = locator.Resolve<IGameHost>();
                    context.Heap.SetValue(DefaultConfigurationSteps.ContextKeys.Game, game);
                    context.Heap.SetValue(DefaultConfigurationSteps.ContextKeys.Host, host);
                }));

            return new GameConfigurator<THost>(configurator);
		}

	    public static IGameConfigurator<THost> SetupGameParameters<THost>([NotNull] this IGameConfigurator<THost> configurator,
	        [NotNull] Func<IGameParameters> parametersFactory)
	        where THost : IGameHost
	    {
	        if (parametersFactory == null) throw new ArgumentNullException(nameof(parametersFactory));

	        var registerPhase = configurator.GetStep(DefaultConfigurationSteps.Registration);
	        registerPhase.AddAction(new PipelineAction(
	            DefaultConfigurationSteps.RegistrationActions.GameParameters,
	            true,
	            context =>
	            {
	                var registrator = GetObjectFromContext<IServiceRegistrator>(context, DefaultConfigurationSteps.ContextKeys.Container);
	                var gameParameters = GetFromFactory(parametersFactory);
	                registrator.RegisterInstance(gameParameters);
	            }));

	        return configurator;
	    }
    }
}
