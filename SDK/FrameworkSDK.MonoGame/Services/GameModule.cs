using FrameworkSDK.DependencyInjection;
using FrameworkSDK.MonoGame.Config;
using FrameworkSDK.MonoGame.Core;
using FrameworkSDK.MonoGame.ExternalComponents;
using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline.Processing;
using FrameworkSDK.MonoGame.Graphics.Services;
using FrameworkSDK.MonoGame.InputManagement;
using FrameworkSDK.MonoGame.InputManagement.Implementations;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.Resources;
using FrameworkSDK.MonoGame.Resources.Generation;
using FrameworkSDK.MonoGame.Services.Implementations;

namespace FrameworkSDK.MonoGame.Services
{
    internal class GameModule<TGame> : IServicesModule where TGame : GameApp
    {
        public void RegisterServices(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.RegisterType<GameApp, TGame>();
            serviceRegistrator.RegisterType<IExternalGameComponentsService, FakeExternalGameComponentsService>();
            serviceRegistrator.RegisterType<IAppTerminator, DefaultAppTerminator>();
            
            //Graphics
            serviceRegistrator.RegisterType<IGraphicsPipelineFactoryService, GraphicsPipelineFactoryService>();
            serviceRegistrator.RegisterType<IGraphicsPipelinePassAssociateService, GraphicsPipelinePassAssociateService>();
            serviceRegistrator.RegisterType<IRenderTargetsFactoryService, RenderTargetsFactoryService>();
            serviceRegistrator.RegisterType<IDisplayService, DisplayService>();
            serviceRegistrator.RegisterType<IIndicesBuffersFactory, Int16IndicesBuffersFactory>();
            serviceRegistrator.RegisterType<IIndicesBuffersFiller, Int16IndicesBuffersFiller>();

            //Resources
            serviceRegistrator.RegisterType<IContentContainersFactory, ContentContainersFactory>();
            serviceRegistrator.RegisterType<IResourceReferencesService, ResourceReferencesService>();
            serviceRegistrator.RegisterType<IResourcesService, ResourcesService>();
            serviceRegistrator.RegisterType<ITextureGeneratorInternal, TextureGeneratorInternal>();
            serviceRegistrator.RegisterType<IRenderTargetsFactory, RenderTargetsFactory>();
         
            //Mvc
            serviceRegistrator.RegisterType<IScenesController, ScenesController>();
            serviceRegistrator.RegisterType<IMvcStrategyService, EmptyMvcStrategyService>();

            //Input
            var inputService = new InputService();
            var inputManager = new InputManager(inputService);
            serviceRegistrator.RegisterInstance<IInputService>(inputService);
            serviceRegistrator.RegisterInstance<IInputManager>(inputManager);
            
            //Camera
            serviceRegistrator.RegisterType<DefaultCamera3DService, DefaultCamera3DService>();
            serviceRegistrator.RegisterFactory(typeof(ICamera3DService), (locator, type) => locator.Resolve(typeof(DefaultCamera3DService)));
            serviceRegistrator.RegisterFactory(typeof(ICamera3DProvider), (locator, type) => locator.Resolve(typeof(DefaultCamera3DService)));
            
            serviceRegistrator.RegisterType<IDebugInfoService, DefaultDebugInfoService>();
            serviceRegistrator.RegisterType<IGameParameters, DefaultGameParameters>();
            serviceRegistrator.RegisterType<IAppStateService, AppStateService>();
            serviceRegistrator.RegisterType<IGameHeartServices, GameHeartServicesHolder>();
            serviceRegistrator.RegisterType<IGameHeart, GameHeart>();
        }
    }
}