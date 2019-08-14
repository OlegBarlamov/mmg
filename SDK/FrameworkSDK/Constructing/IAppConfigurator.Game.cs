using FrameworkSDK.IoC;
using FrameworkSDK.Modules;
using JetBrains.Annotations;

namespace FrameworkSDK.Constructing
{
	public static partial class AppConfiguratorExtensions
	{
		public static IGameConfigurator<THost> UseGameFramework<THost>([NotNull] this IAppConfigurator configurator) where THost : IGameHost
		{
		    var registerPhase = configurator.GetPhase(DefaultConfigurationSteps.Registration);
		    registerPhase.AddAction(new SimpleConfigurationAction(
                DefaultConfigurationSteps.RegistrationActions.Game,
                true,
                context =>
                {
                    var registrator = GetObjectFromContext<IServiceRegistrator>(context, DefaultConfigurationSteps.ContextKeys.Container);
                    var module = new GameModule<THost>();
                    registrator.RegisterModule(module);
                }));

		    var constructPhase = configurator.GetPhase(DefaultConfigurationSteps.Constructing);
            constructPhase.AddAction(new SimpleConfigurationAction(
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
	}

    public static class GameConfiguratorExtensions
    {
        public static IGameConfigurator<THost> SetupGameParameters<THost>([NotNull] this IGameConfigurator<THost> configurator)
            where THost : IGameHost
        {

        }
    }
}
