using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public class SetRenderTargetToDisplayAction : GraphicsPipelineActionBase
    {
        public SetRenderTargetToDisplayAction([NotNull] string name) : base(name)
        {
        }
        
        public override void Process(GameTime gameTime, IGraphicDeviceContext graphicDeviceContext, IReadOnlyList<IGraphicComponent> associatedComponents)
        {
            graphicDeviceContext.SetRenderTargetToDisplay();
        }
    }
}