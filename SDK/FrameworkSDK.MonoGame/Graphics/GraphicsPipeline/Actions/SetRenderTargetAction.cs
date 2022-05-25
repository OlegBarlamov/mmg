using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public class SetRenderTargetAction : IGraphicsPipelineAction
    {
        public string Name { get; }
        public RenderTarget2D RenderTarget { get; }
        public bool IsDisabled { get; set; }

        public SetRenderTargetAction([NotNull] string name, [NotNull] RenderTarget2D renderTarget)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            RenderTarget = renderTarget ?? throw new ArgumentNullException(nameof(renderTarget));
        }
        
        public void Process(GameTime gameTime, IGraphicDeviceContext graphicDeviceContext, IReadOnlyList<IGraphicComponent> associatedComponents)
        {
            graphicDeviceContext.SetRenderTarget(RenderTarget);
        }
    }
}