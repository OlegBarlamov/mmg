using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.IoC;
using FrameworkSDK.MonoGame.Config;
using FrameworkSDK.MonoGame.Core;
using FrameworkSDK.MonoGame.ExternalComponents;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline.Processing;
using FrameworkSDK.MonoGame.Graphics.Services;
using FrameworkSDK.MonoGame.InputManagement;
using FrameworkSDK.MonoGame.InputManagement.Implementations;
using FrameworkSDK.MonoGame.Resources;
using FrameworkSDK.MonoGame.Resources.Generation;
using FrameworkSDK.MonoGame.Services;
using FrameworkSDK.MonoGame.Services.Implementations;

namespace FrameworkSDK.MonoGame.Constructing
{
    internal class GameModule<TGameHost> : IServicesModule where TGameHost : GameApp
    {
        public void Register(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.RegisterType<IExternalGameComponentsService, FakeExternalGameComponentsService>();
            
            //Graphics
            serviceRegistrator.RegisterType<IGraphicsPipelineFactoryService, GraphicsPipelineFactoryService>();
            serviceRegistrator.RegisterType<IGraphicsPipelineProcessor, DefaultGraphicsPipelineProcessor>();
            serviceRegistrator.RegisterType<IGraphicsPipelinePassAssociateService, GraphicsPipelinePassAssociateService>();
            serviceRegistrator.RegisterType<IRenderTargetsFactoryService, RenderTargetsFactoryService>();
            serviceRegistrator.RegisterType<IDisplayService, DisplayService>();

            //Resources
            serviceRegistrator.RegisterType<IContentContainersFactory, ContentContainersFactory>();
            serviceRegistrator.RegisterType<IResourceReferencesService, ResourceReferencesService>();
            serviceRegistrator.RegisterType<IResourcesService, ResourcesService>();
            serviceRegistrator.RegisterType<ITextureGeneratorService, TextureGeneratorService>();
            serviceRegistrator.RegisterType<IRenderTargetsFactory, RenderTargetsFactory>();
         
            //Mvc
            serviceRegistrator.RegisterType<IScenesController, ScenesController>();
            serviceRegistrator.RegisterType<IViewsProvider, DefaultViewsProvider>();
            serviceRegistrator.RegisterType<IControllersProvider, DefaultControllersProvider>();
            serviceRegistrator.RegisterType<IModelsProvider, DefaultModelsProvider>();
            serviceRegistrator.RegisterType<IViewsResolver, DefaultViewsResolver>();
            serviceRegistrator.RegisterType<IControllersResolver, DefaultControllersResolver>();
            serviceRegistrator.RegisterType<IModelsResolver, DefaultModelsResolver>();
            serviceRegistrator.RegisterType<IMvcStrategyService, DefaultMvcStrategy>();
            serviceRegistrator.RegisterType<IScenesContainer, DefaultScenesContainer>();
            
            //Input
            var inputService = new InputService();
            var inputManager = new InputManager(inputService);
            serviceRegistrator.RegisterInstance<IInputService>(inputService);
            serviceRegistrator.RegisterInstance<IInputManager>(inputManager);
            
            serviceRegistrator.RegisterType<IGameParameters, DefaultGameParameters>();
            serviceRegistrator.RegisterType<IAppStateService, AppStateService>();
            serviceRegistrator.RegisterType<IGameHostProvider, GameHostProviderService<TGameHost>>();
            serviceRegistrator.RegisterType<IGameHeartServices, GameHeartServicesHolder>();
            serviceRegistrator.RegisterType<IGameHeart, GameHeart>();
        }
    }
}