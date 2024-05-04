using System;
using FrameworkSDK.Logging;
using FrameworkSDK.MonoGame.Core;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.Camera2D;
using FrameworkSDK.MonoGame.Graphics.Camera3D;
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
        [NotNull] private IFrameworkLogger FrameworkLogger { get; }
        [NotNull] private ICamera3DService Camera3DService { get; }
        [NotNull] private IDebugInfoService DebugInfoService { get; }
        [NotNull] private IIndicesBuffersFiller IndicesBuffersFiller { get; }
        [NotNull] private IVideoBuffersFactoryService VideoBuffersFactoryService { get; }
        [NotNull] private ICamera2DService Camera2DService { get; }

        public GraphicsPipelineFactoryService(
            [NotNull] IGameHeartServices gameHeartServices,
            [NotNull] IRenderTargetsFactoryService renderTargetsFactoryService,
            [NotNull] IDisplayService displayService,
            [NotNull] IGraphicsPipelinePassAssociateService graphicsPipelinePassAssociateService,
            [NotNull] IFrameworkLogger frameworkLogger,
            [NotNull] ICamera3DService camera3DService,
            [NotNull] IDebugInfoService debugInfoService,
            [NotNull] IIndicesBuffersFiller indicesBuffersFiller,
            [NotNull] IVideoBuffersFactoryService videoBuffersFactoryService,
            [NotNull] ICamera2DService camera2DService)
        {
            GameHeartServices = gameHeartServices ?? throw new ArgumentNullException(nameof(gameHeartServices));
            RenderTargetsFactoryService = renderTargetsFactoryService ?? throw new ArgumentNullException(nameof(renderTargetsFactoryService));
            DisplayService = displayService ?? throw new ArgumentNullException(nameof(displayService));
            GraphicsPipelinePassAssociateService = graphicsPipelinePassAssociateService ?? throw new ArgumentNullException(nameof(graphicsPipelinePassAssociateService));
            FrameworkLogger = frameworkLogger ?? throw new ArgumentNullException(nameof(frameworkLogger));
            Camera3DService = camera3DService ?? throw new ArgumentNullException(nameof(camera3DService));
            DebugInfoService = debugInfoService ?? throw new ArgumentNullException(nameof(debugInfoService));
            IndicesBuffersFiller = indicesBuffersFiller ?? throw new ArgumentNullException(nameof(indicesBuffersFiller));
            VideoBuffersFactoryService = videoBuffersFactoryService ?? throw new ArgumentNullException(nameof(videoBuffersFactoryService));
            Camera2DService = camera2DService ?? throw new ArgumentNullException(nameof(camera2DService));
        }

        public IGraphicsPipelineBuilder Create(IReadOnlyObservableList<IGraphicComponent> graphicComponents)
        {
            return new GraphicsPipelineBuilder(
                graphicComponents,
                GraphicsPipelinePassAssociateService,
                FrameworkLogger,
                DebugInfoService,
                RenderTargetsFactoryService,
                VideoBuffersFactoryService
                );
        }

        public IGraphicDeviceContext CreateGraphicDeviceContext()
        {
            return new PipelineGraphicDeviceContext(
                GameHeartServices.SpriteBatch,
                GameHeartServices.GraphicsDeviceManager.GraphicsDevice,
                DisplayService,
                Camera3DService,
                DebugInfoService,
                IndicesBuffersFiller,
                Camera2DService
                );
        }
    }
}