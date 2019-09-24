using System;
using FrameworkSDK.Constructing;
using FrameworkSDK.MonoGame.GameStructure;
using FrameworkSDK.IoC;
using FrameworkSDK.Localization;
using FrameworkSDK.Pipelines;
using JetBrains.Annotations;
using NetExtensions;

namespace FrameworkSDK.MonoGame.Constructing
{
	public static class GameConfiguratorExtensions
	{
		internal static IGameConfigurator<THost> UseGameFramework<THost>([NotNull] this IAppConfigurator configurator) where THost : IGameHost
		{
		    var registerPhase = configurator.GetStep(GameDefaultConfigurationSteps.Registration);
		    registerPhase.AddAction(new SimplePipelineAction(
		        GameDefaultConfigurationSteps.RegistrationActions.Game,
                true,
                context =>
                {
                    var registrator = GetObjectFromContext<IServiceRegistrator>(context, GameDefaultConfigurationSteps.ContextKeys.Container);
                    var module = new GameModule<THost>();
                    registrator.RegisterModule(module);
                }));

		    var constructPhase = configurator.GetStep(GameDefaultConfigurationSteps.Constructing);
            constructPhase.AddAction(new SimplePipelineAction(
                GameDefaultConfigurationSteps.ConstructingActions.Game,
                true,
                context =>
                {
                    var locator = GetObjectFromContext<IServiceLocator>(context, GameDefaultConfigurationSteps.ContextKeys.Locator);
                    var game = locator.Resolve<IGame>();
                    var host = locator.Resolve<IGameHost>();
                    context.Heap.SetValue(GameDefaultConfigurationSteps.ContextKeys.Game, game);
                    context.Heap.SetValue(GameDefaultConfigurationSteps.ContextKeys.Host, host);
                }));

            return new GameConfigurator<THost>(configurator);
		}

	    public static IGameConfigurator<THost> SetupGameParameters<THost>([NotNull] this IGameConfigurator<THost> configurator,
	        [NotNull] Func<IGameParameters> parametersFactory)
	        where THost : IGameHost
	    {
	        if (parametersFactory == null) throw new ArgumentNullException(nameof(parametersFactory));

	        var registerPhase = configurator.GetStep(GameDefaultConfigurationSteps.Registration);
	        registerPhase.AddAction(new SimplePipelineAction(
	            GameDefaultConfigurationSteps.RegistrationActions.GameParameters,
	            true,
	            context =>
	            {
	                var registrator = GetObjectFromContext<IServiceRegistrator>(context, GameDefaultConfigurationSteps.ContextKeys.Container);
	                var gameParameters = GetFromFactory(parametersFactory);
	                registrator.RegisterInstance(gameParameters);
	            }));

	        return configurator;
	    }

	    [NotNull]
	    private static T GetFromFactory<T>([NotNull] Func<T> factory)
	    {
	        if (factory == null) throw new ArgumentNullException(nameof(factory));

	        var result = factory.Invoke();
	        if (result == null)
	            throw new GameConstructingException(Strings.Exceptions.Constructing.FactoryObjectNull);

	        return result;
	    }

	    [NotNull]
	    private static T GetObjectFromContext<T>(IPipelineContext context, string key) where T : class
	    {
	        return context.Heap.GetObject<T>(key) ?? throw new GameConstructingException(
	                   string.Format(Strings.Exceptions.Constructing.ObjectInContextNotFound, key, typeof(T).Name));
	    }
    }
}
