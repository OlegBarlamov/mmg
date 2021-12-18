using System;
using FrameworkSDK.MonoGame.Core;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline.Processing;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using FrameworkSDK.MonoGame.Graphics.Services;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    [UsedImplicitly]
    internal class GraphicsPipelineFactoryService : IGraphicsPipelineFactoryService
    {
        [NotNull] private IGameHeartServices GameHeartServices { get; }
        [NotNull] private IRenderTargetsFactoryService RenderTargetsFactoryService { get; }
        [NotNull] private IDisplayService DisplayService { get; }
        [NotNull] private IGraphicsPipelineProcessorsFactory GraphicsPipelineProcessorsFactory { get; }

        public GraphicsPipelineFactoryService(
            [NotNull] IGameHeartServices gameHeartServices,
            [NotNull] IRenderTargetsFactoryService renderTargetsFactoryService,
            [NotNull] IDisplayService displayService,
            [NotNull] IGraphicsPipelineProcessorsFactory graphicsPipelineProcessorsFactory)
        {
            GameHeartServices = gameHeartServices ?? throw new ArgumentNullException(nameof(gameHeartServices));
            RenderTargetsFactoryService = renderTargetsFactoryService ?? throw new ArgumentNullException(nameof(renderTargetsFactoryService));
            DisplayService = displayService ?? throw new ArgumentNullException(nameof(displayService));
            GraphicsPipelineProcessorsFactory = graphicsPipelineProcessorsFactory ?? throw new ArgumentNullException(nameof(graphicsPipelineProcessorsFactory));
        }
        
        public IGraphicsPipelineBuilder Create(IGraphicsPipeline graphicsPipeline = null)
        {
            var graphicDeviceContext = CreateGraphicDeviceContext();
            var renderContext = CreateRenderContext();
            var drawContext = CreateDrawContext();

            var processor = GraphicsPipelineProcessorsFactory.Create();
            var targetPipeline = graphicsPipeline ?? new DefaultGraphicsPipeline(
                                     processor,
                                     graphicDeviceContext,
                                     RenderTargetsFactoryService,
                                     DisplayService,
                                     renderContext);
            
            return new GraphicsPipelineBuilder(
                targetPipeline,
                graphicDeviceContext,
                drawContext,
                renderContext);
        }
        
        public IDrawContext CreateDrawContext()
        {
            return new DrawContext(GameHeartServices.SpriteBatch);
        }

        public IGraphicDeviceContext CreateGraphicDeviceContext()
        {
            return new PipelineGraphicDeviceContext(GameHeartServices.SpriteBatch, GameHeartServices.GraphicsDeviceManager.GraphicsDevice);
        }

        public IRenderContext CreateRenderContext()
        {
            return new RenderContext();
        }
    }
}