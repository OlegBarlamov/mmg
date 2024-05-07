using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public class DrawRenderTargetToDisplay : GraphicsPipelineActionBase
    {
        public BeginDrawConfig BeginDrawConfig { get; }
        public IRenderTargetWrapper RenderTargetWrapper { get; }

        public DrawRenderTargetToDisplay([NotNull] string name, [NotNull] BeginDrawConfig beginDrawConfig,
            [NotNull] IRenderTargetWrapper renderTargetWrapper) : base(name)
        {
            BeginDrawConfig = beginDrawConfig ?? throw new ArgumentNullException(nameof(beginDrawConfig));
            RenderTargetWrapper = renderTargetWrapper ?? throw new ArgumentNullException(nameof(renderTargetWrapper));
        }
        
        public override void Process(GameTime gameTime, IGraphicDeviceContext graphicDeviceContext, IReadOnlyList<IGraphicComponent> associatedComponents)
        {
            graphicDeviceContext.BeginDraw(BeginDrawConfig);
            graphicDeviceContext.SetRenderTargetToDisplay();
            graphicDeviceContext.DrawContext.Draw(RenderTargetWrapper.RenderTarget, DrawParameters.StretchToFullScreen(graphicDeviceContext.DisplayService));
            graphicDeviceContext.EndDraw();
        }
    }
}