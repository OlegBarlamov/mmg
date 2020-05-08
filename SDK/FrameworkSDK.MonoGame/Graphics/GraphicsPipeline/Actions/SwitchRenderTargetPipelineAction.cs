using System;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public class SwitchRenderTargetPipelineAction : SystemPipelineAction
    {
        private Func<RenderTarget2D> RenderTargetFunc { get; }
        [CanBeNull] private Color? ClearColor { get; }

        public SwitchRenderTargetPipelineAction([NotNull] string name, [NotNull] Func<RenderTarget2D> renderTargetProvider,
            [CanBeNull] Color? clearColor = null, [CanBeNull] IGraphicDeviceContext context = null)
            : base(name, context)
        {
            RenderTargetFunc = renderTargetProvider ?? throw new ArgumentNullException(nameof(renderTargetProvider));
            ClearColor = clearColor;
        }
        
        public SwitchRenderTargetPipelineAction([NotNull] string name, [CanBeNull] RenderTarget2D renderTarget,
            [CanBeNull] Color? clearColor = null, [CanBeNull] IGraphicDeviceContext context = null)
            : this(name, () => renderTarget, clearColor, context)
        {
        }

        protected override void Process(GameTime gameTime, IGraphicDeviceContext context)
        {
            context.SetRenderTarget(RenderTargetFunc());
            if (ClearColor != null)
                context.Clear(ClearColor.Value);
        }
    }
}