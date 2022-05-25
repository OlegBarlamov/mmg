using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public interface IGraphicsPipeline : IDisposable
    {
        IReadOnlyList<IGraphicsPipelineAction> Actions { get; }
        void Process(GameTime gameTime, IGraphicDeviceContext graphicDeviceContext);
    }
}