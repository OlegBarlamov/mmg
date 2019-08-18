using FrameworkSDK.Game;
using FrameworkSDK.Game.Mapping;
using FrameworkSDK.Game.Mapping.Default;
using FrameworkSDK.Game.Scenes;
using FrameworkSDK.IoC;
using FrameworkSDK.Services.Graphics;

namespace FrameworkSDK.Modules
{
	internal class GameModule<TGameHost> : IServicesModule where TGameHost : IGameHost
	{
		public void Register(IServiceRegistrator serviceRegistrator)
		{
			serviceRegistrator.RegisterType<IScenesController, ScenesController>();
			serviceRegistrator.RegisterType<IViewsProvider, DefaultViewsProvider>();
			serviceRegistrator.RegisterType<IControllersProvider, DefaultControllersProvider>();
			serviceRegistrator.RegisterType<IModelsProvider, DefaultModelsProvider>();
			serviceRegistrator.RegisterType<IViewsResolver, DefaultViewsResolver>();
			serviceRegistrator.RegisterType<IControllersResolver, DefaultControllersResolver>();
			serviceRegistrator.RegisterType<IModelsResolver, DefaultModelsResolver>();
			serviceRegistrator.RegisterType<IMvcStrategyService, DefaultMvcStrategy>();

            serviceRegistrator.RegisterType<ISpriteBatchProvider, DefaultSpriteBatchProvider>();
            serviceRegistrator.RegisterType<IGameParameters, DefaultGameParameters>();

            serviceRegistrator.RegisterType<IGameHost, TGameHost>();
		    serviceRegistrator.RegisterType<IGame, GameShell>();
		}
	}
}
