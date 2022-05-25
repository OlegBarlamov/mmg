using System;
using FrameworkSDK.MonoGame.Core;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline.Processing;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using FrameworkSDK.MonoGame.Graphics.Services;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using NetExtensions.Collections;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    [UsedImplicitly]
    internal class GraphicsPipelineFactoryService : IGraphicsPipelineFactoryService
    {
        [NotNull] private IGameHeartServices GameHeartServices { get; }
        [NotNull] private IRenderTargetsFactoryService RenderTargetsFactoryService { get; }
        [NotNull] private IDisplayService DisplayService { get; }
        [NotNull] private IGraphicsPipelinePassAssociateService GraphicsPipelinePassAssociateService { get; }

        public GraphicsPipelineFactoryService(
            [NotNull] IGameHeartServices gameHeartServices,
            [NotNull] IRenderTargetsFactoryService renderTargetsFactoryService,
            [NotNull] IDisplayService displayService,
            [NotNull] IGraphicsPipelinePassAssociateService graphicsPipelinePassAssociateService)
        {
            GameHeartServices = gameHeartServices ?? throw new ArgumentNullException(nameof(gameHeartServices));
            RenderTargetsFactoryService = renderTargetsFactoryService ?? throw new ArgumentNullException(nameof(renderTargetsFactoryService));
            DisplayService = displayService ?? throw new ArgumentNullException(nameof(displayService));
            GraphicsPipelinePassAssociateService = graphicsPipelinePassAssociateService ?? throw new ArgumentNullException(nameof(graphicsPipelinePassAssociateService));
        }

        public IGraphicsPipelineBuilder Create(IReadOnlyObservableList<IGraphicComponent> graphicComponents)
        {
            return new GraphicsPipelineBuilder(graphicComponents, GraphicsPipelinePassAssociateService);
        }

        public IDrawContext CreateDrawContext()
        {
            return new DrawContext(GameHeartServices.SpriteBatch);
        }

        public IGraphicDeviceContext CreateGraphicDeviceContext()
        {
            return new PipelineGraphicDeviceContext(GameHeartServices.SpriteBatch, GameHeartServices.GraphicsDeviceManager.GraphicsDevice, DisplayService);
        }

        public IRenderContext CreateRenderContext()
        {
            return new RenderContext(GameHeartServices.GraphicsDeviceManager.GraphicsDevice);
        }
    }
}