using System;
using FrameworkSDK.DependencyInjection;
using FrameworkSDK.Logging;
using FrameworkSDK.MonoGame.Core;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline.Processing;
using FrameworkSDK.MonoGame.Graphics.Materials;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Constructing
{
    [UsedImplicitly]
    internal class GameAppComponent : IAppSubSystem
    {
        private IServiceLocator ServiceLocator { get; }
        private IFrameworkLogger Logger { get; }
        private IGraphicsPipelinePassAssociateService GraphicsPipelinePassAssociateService { get; }
        private IDebugInfoService DebugInfoService { get; }
        private IGameHeart GameHeart { get; set; }

        public GameAppComponent(
            [NotNull] IServiceLocator serviceLocator,
            [NotNull] IFrameworkLogger logger,
            [NotNull] IGraphicsPipelinePassAssociateService graphicsPipelinePassAssociateService,
            [NotNull] IDebugInfoService debugInfoService)
        {
            ServiceLocator = serviceLocator ?? throw new ArgumentNullException(nameof(serviceLocator));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            GraphicsPipelinePassAssociateService = graphicsPipelinePassAssociateService ?? throw new ArgumentNullException(nameof(graphicsPipelinePassAssociateService));
            DebugInfoService = debugInfoService ?? throw new ArgumentNullException(nameof(debugInfoService));
            AppContext.Initialize(Logger, ServiceLocator);
        }
        
        public void Configure()
        {
            GameHeart = ServiceLocator.Resolve<IGameHeart>();
        }

        public void Run()
        {
            DebugInfoService.StartTimer("App time");
            GameHeart.Run();
        }

        public void Dispose()
        {
        }
    }
}