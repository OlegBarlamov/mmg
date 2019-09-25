using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.IoC;
using FrameworkSDK.MonoGame.Config;
using FrameworkSDK.MonoGame.Graphics.Pipeline;

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

            serviceRegistrator.RegisterType<IGameParameters, DefaultGameParameters>();
            serviceRegistrator.RegisterType<IGraphicsPipeline, GraphicsPipeline>();
            serviceRegistrator.RegisterType<IComponentsByPassAggregator, ComponentsByPassAggregator>();
            serviceRegistrator.RegisterType<IGraphicsPipelineContextFactory, GraphicsPipelineContextFactory>();

            serviceRegistrator.RegisterType<IGameHost, TGameHost>();
            serviceRegistrator.RegisterType<IGameHeart, GameHeart>();
        }
    }
}