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
        public IRenderTargetWrapper Wrapper { get; }

        public SetRenderTargetAction([NotNull] string name, [NotNull] IRenderTargetWrapper renderTargetWrapper) : base(name)
        {
            Wrapper = renderTargetWrapper ?? throw new ArgumentNullException(nameof(renderTargetWrapper));
        }
        
        public override void Process(GameTime gameTime, IGraphicDeviceContext graphicDeviceContext, IReadOnlyList<IGraphicComponent> associatedComponents)
        {
            graphicDeviceContext.SetRenderTarget(Wrapper.RenderTarget);
        }
    }
}