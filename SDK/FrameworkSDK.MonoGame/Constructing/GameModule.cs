using FrameworkSDK.MonoGame.GameStructure.Mapping;
using FrameworkSDK.MonoGame.GameStructure.Mapping.Default;
using FrameworkSDK.MonoGame.GameStructure.Scenes;
using FrameworkSDK.IoC;
using FrameworkSDK.MonoGame.GameStructure;
using FrameworkSDK.MonoGame.Services.Graphics;

namespace FrameworkSDK.MonoGame.Constructing
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
            serviceRegistrator.RegisterType<IScenesContainer, DefaultScenesContainer>();

            serviceRegistrator.RegisterType<ISpriteBatchProvider, DefaultSpriteBatchProvider>();
            serviceRegistrator.RegisterType<IGameParameters, DefaultGameParameters>();
            serviceRegistrator.RegisterType<IGraphicsPipelineFactory, DefaultGraphicsPipelineFactory>();

            serviceRegistrator.RegisterType<IGameHost, TGameHost>();
            serviceRegistrator.RegisterType<IGame, GameShell>();
        }
    }
}