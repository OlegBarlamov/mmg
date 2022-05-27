using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public class EmptyGraphicsPipeline : IGraphicsPipeline
    {
        public void Dispose()
        {
            
        }

        public IReadOnlyList<IGraphicsPipelineAction> Actions { get; } = Array.Empty<IGraphicsPipelineAction>();
        public void Process(GameTime gameTime, IGraphicDeviceContext graphicDeviceContext)
        {
        }
    }
}