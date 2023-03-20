using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline.Presets
{
    public class GraphicsPipeline2DDrawingPreset : GraphicPipelineBuilderWrapper, IGraphicsPipelineBuilder, IDisposable
    {
        public static class PipelineActions
        {
            public static string DrawComponents = "Default";
            public static string DrawDebugComponents = "Debug";
            public static string DrawUI = "UI";
            public static string DrawDebugUI = "Debug_UI";
        }
        
        public BeginDrawConfig BeginDrawConfig { get; }

        private readonly IRenderTargetWrapper _defaultGraphicsPipelineRenderTarget;

        internal GraphicsPipeline2DDrawingPreset([NotNull] IGraphicsPipelineBuilder builder, [NotNull] BeginDrawConfig beginDrawConfig)
            : base(builder)
        {
            BeginDrawConfig = beginDrawConfig ?? throw new ArgumentNullException(nameof(beginDrawConfig));
            
            _defaultGraphicsPipelineRenderTarget = Builder.RenderTargetsFactoryService.CreateFullScreenRenderTarget(
                false,
                SurfaceFormat.Color,
                DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            Builder
                .SetRenderTarget(_defaultGraphicsPipelineRenderTarget.RenderTarget)
                .BeginDraw(BeginDrawConfig)
                .DrawComponents(PipelineActions.DrawComponents)
                .DrawComponents(PipelineActions.DrawDebugComponents)
                .DrawComponents(context => context.Camera2DProvider.GetScreenCamera(), PipelineActions.DrawUI)
                .DrawComponents(context => context.Camera2DProvider.GetScreenCamera(), PipelineActions.DrawDebugUI);
        }

        public void AddActionBeforeFinalDraw(IGraphicsPipelineAction action)
        {
            Builder.AddAction(action);
        }

        public override IGraphicsPipeline Build(IDisposable resources = null)
        {
            return Builder
                .EndDraw()
                .DrawRenderTargetToDisplay(_defaultGraphicsPipelineRenderTarget.RenderTarget)
                .Build(this);
        }

        public void Dispose()
        {
            _defaultGraphicsPipelineRenderTarget.Dispose();
        }
    }
}