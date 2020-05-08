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
        [NotNull] private IGraphicsPipelineProcessor GraphicsPipelineProcessor { get; }
        [NotNull] private IRenderTargetsFactoryService RenderTargetsFactoryService { get; }
        [NotNull] private IDisplayService DisplayService { get; }

        public GraphicsPipelineFactoryService(
            [NotNull] IGameHeartServices gameHeartServices,
            [NotNull] IGraphicsPipelineProcessor graphicsPipelineProcessor,
            [NotNull] IRenderTargetsFactoryService renderTargetsFactoryService,
            [NotNull] IDisplayService displayService)
        {
            GameHeartServices = gameHeartServices ?? throw new ArgumentNullException(nameof(gameHeartServices));
            GraphicsPipelineProcessor = graphicsPipelineProcessor ?? throw new ArgumentNullException(nameof(graphicsPipelineProcessor));
            RenderTargetsFactoryService = renderTargetsFactoryService ?? throw new ArgumentNullException(nameof(renderTargetsFactoryService));
            DisplayService = displayService ?? throw new ArgumentNullException(nameof(displayService));
        }
        
        public IGraphicsPipelineBuilder Create(IGraphicsPipeline graphicsPipeline = null)
        {
            var graphicDeviceContext = CreateGraphicDeviceContext();
            var targetPipeline = graphicsPipeline ?? new DefaultGraphicsPipeline(GraphicsPipelineProcessor, graphicDeviceContext, RenderTargetsFactoryService, DisplayService);
            return new GraphicsPipelineBuilder(
                targetPipeline,
                graphicDeviceContext,
                CreateDrawContext(),
                CreateRenderContext());
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