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
        public RenderTarget2D RenderTarget2D { get; }

        public DrawRenderTargetToDisplay([NotNull] string name, [NotNull] BeginDrawConfig beginDrawConfig,
            [NotNull] RenderTarget2D renderTarget2D) : base(name)
        {
            BeginDrawConfig = beginDrawConfig ?? throw new ArgumentNullException(nameof(beginDrawConfig));
            RenderTarget2D = renderTarget2D ?? throw new ArgumentNullException(nameof(renderTarget2D));
        }
        
        public override void Process(GameTime gameTime, IGraphicDeviceContext graphicDeviceContext, IReadOnlyList<IGraphicComponent> associatedComponents)
        {
            graphicDeviceContext.BeginDraw(BeginDrawConfig);
            graphicDeviceContext.SetRenderTargetToDisplay();
            graphicDeviceContext.DrawContext.Draw(RenderTarget2D, DrawParameters.StretchToFullScreen(graphicDeviceContext.DisplayService));
            graphicDeviceContext.EndDraw();
        }
    }
}