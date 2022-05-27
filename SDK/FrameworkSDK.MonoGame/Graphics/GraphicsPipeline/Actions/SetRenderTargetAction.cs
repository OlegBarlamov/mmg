using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public class SetRenderTargetAction : GraphicsPipelineActionBase
    {
        public RenderTarget2D RenderTarget { get; }

        public SetRenderTargetAction([NotNull] string name, [NotNull] RenderTarget2D renderTarget) : base(name)
        {
            RenderTarget = renderTarget ?? throw new ArgumentNullException(nameof(renderTarget));
        }
        
        public override void Process(GameTime gameTime, IGraphicDeviceContext graphicDeviceContext, IReadOnlyList<IGraphicComponent> associatedComponents)
        {
            graphicDeviceContext.SetRenderTarget(RenderTarget);
        }
    }
}