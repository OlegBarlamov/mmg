using System;
using FrameworkSDK.DependencyInjection;
using FrameworkSDK.Logging;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline.Processing;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Constructing
{
    [UsedImplicitly]
    internal class GameAppComponent : IAppSubSystem
    {
        private IServiceLocator ServiceLocator { get; }
        private IFrameworkLogger Logger { get; }
        private IGraphicsPipelinePassAssociateService GraphicsPipelinePassAssociateService { get; }
        private IGameHeart GameHeart { get; set; }

        public GameAppComponent(
            [NotNull] IServiceLocator serviceLocator,
            [NotNull] IFrameworkLogger logger,
            [NotNull] IGraphicsPipelinePassAssociateService graphicsPipelinePassAssociateService)
        {
            ServiceLocator = serviceLocator ?? throw new ArgumentNullException(nameof(serviceLocator));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            GraphicsPipelinePassAssociateService = graphicsPipelinePassAssociateService ?? throw new ArgumentNullException(nameof(graphicsPipelinePassAssociateService));
            AppContext.Initialize(Logger, ServiceLocator);
        }
        
        public void Configure()
        {
            GraphicsPipelinePassAssociateService.Initialize();
            GameHeart = ServiceLocator.Resolve<IGameHeart>();
        }

        public void Run()
        {
            GameHeart.Run();
        }

        public void Dispose()
        {
        }
    }
}