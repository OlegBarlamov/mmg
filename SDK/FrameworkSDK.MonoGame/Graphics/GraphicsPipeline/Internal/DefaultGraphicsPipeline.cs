using System;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline.Processing;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using FrameworkSDK.MonoGame.Graphics.Services;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    internal class DefaultGraphicsPipeline : ThreePhaseGraphicsPipeline
    {
        private readonly IRenderTargetWrapper _renderTargetWrapper;
        
        public DefaultGraphicsPipeline(
            [NotNull] IGraphicsPipelineProcessor graphicsPipelineProcessor,
            [NotNull] IGraphicDeviceContext graphicDeviceContext,
            [NotNull] IRenderTargetsFactoryService renderTargetsFactoryService,
            [NotNull] IDisplayService displayService,
            [NotNull] IRenderContext renderContext)
            : base(graphicsPipelineProcessor)
        {
            if (graphicsPipelineProcessor == null) throw new ArgumentNullException(nameof(graphicsPipelineProcessor));
            if (graphicDeviceContext == null) throw new ArgumentNullException(nameof(graphicDeviceContext));
            if (renderTargetsFactoryService == null) throw new ArgumentNullException(nameof(renderTargetsFactoryService));
            if (displayService == null) throw new ArgumentNullException(nameof(displayService));
            if (renderContext == null) throw new ArgumentNullException(nameof(renderContext));

            _renderTargetWrapper = renderTargetsFactoryService.CreateFullScreenRenderTarget(false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            AddBeginAction(new SwitchRenderTargetPipelineAction("renderTarget", () => _renderTargetWrapper.RenderTarget, Color.LightGreen, graphicDeviceContext));
            
            AddEndAction(new SwitchRenderTargetPipelineAction("switchToDisplay", (RenderTarget2D)null, null, graphicDeviceContext));
            AddEndAction(new BeginDrawPipelineAction("beginDraw", new BeginDrawConfig(), graphicDeviceContext));
            AddEndAction(new DrawRenderTargetAction("drawRenderTarget", () => _renderTargetWrapper.RenderTarget, DrawParameters.StretchToFullScreen(displayService), graphicDeviceContext));
            AddEndAction(new DrawComponentsPipelineAction("default", graphicDeviceContext));
            AddEndAction(new EndDrawPipelineAction("endDraw", graphicDeviceContext));
        }

        public override void Dispose()
        {
            _renderTargetWrapper.Dispose();

            base.Dispose();
        }
    }
}