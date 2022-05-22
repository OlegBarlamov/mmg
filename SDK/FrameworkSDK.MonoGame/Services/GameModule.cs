using FrameworkSDK.DependencyInjection;
using FrameworkSDK.MonoGame.Cameras;
using FrameworkSDK.MonoGame.Config;
using FrameworkSDK.MonoGame.Core;
using FrameworkSDK.MonoGame.ExternalComponents;
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
            
            //Graphics
            serviceRegistrator.RegisterType<IGraphicsPipelineFactoryService, GraphicsPipelineFactoryService>();
            serviceRegistrator.RegisterType<IGraphicsPipelinePassAssociateService, GraphicsPipelinePassAssociateService>();
            serviceRegistrator.RegisterType<IRenderTargetsFactoryService, RenderTargetsFactoryService>();
            serviceRegistrator.RegisterType<IGraphicsPipelineProcessorsFactory, DefaultGraphicsPipelineProcessorFactory>();
            serviceRegistrator.RegisterType<IDisplayService, DisplayService>();

            //Resources
            serviceRegistrator.RegisterType<IContentContainersFactory, ContentContainersFactory>();
            serviceRegistrator.RegisterType<IResourceReferencesService, ResourceReferencesService>();
            serviceRegistrator.RegisterType<IResourcesService, ResourcesService>();
            serviceRegistrator.RegisterType<ITextureGeneratorService, TextureGeneratorService>();
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
            serviceRegistrator.RegisterType<ICameraService, CameraService>();
            
            serviceRegistrator.RegisterType<IGameParameters, DefaultGameParameters>();
            serviceRegistrator.RegisterType<IAppStateService, AppStateService>();
            serviceRegistrator.RegisterType<IGameHeartServices, GameHeartServicesHolder>();
            serviceRegistrator.RegisterType<IGameHeart, GameHeart>();
        }
    }
}