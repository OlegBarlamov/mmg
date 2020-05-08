using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public class DrawRenderTargetAction : DrawPipelineAction
    {
        [NotNull] private Func<RenderTarget2D> RenderTargetProvider { get; }
        [NotNull] private DrawParameters DrawParameters { get; }

        public DrawRenderTargetAction([NotNull] string name, [NotNull] Func<RenderTarget2D> renderTargetProvider, [CanBeNull] DrawParameters drawParameters = null, [CanBeNull] IGraphicDeviceContext context = null)
            : base(name, context)
        {
            RenderTargetProvider = renderTargetProvider ?? throw new ArgumentNullException(nameof(renderTargetProvider));
            DrawParameters = drawParameters ?? new DrawParameters();
        }
        
        public DrawRenderTargetAction([NotNull] string name, [NotNull] RenderTarget2D renderTarget, [CanBeNull] DrawParameters drawParameters = null, [CanBeNull] IGraphicDeviceContext context = null)
            : this(name, () => renderTarget, drawParameters, context)
        {
            if (renderTarget == null) throw new ArgumentNullException(nameof(renderTarget));
        }

        protected override void ProcessDraw(GameTime gameTime, IDrawContext drawContext, IEnumerable<IGraphicComponent> components)
        {
            drawContext.Draw(RenderTargetProvider.Invoke(), DrawParameters);
        }
    }
}