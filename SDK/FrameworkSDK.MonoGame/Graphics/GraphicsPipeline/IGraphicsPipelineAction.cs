using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using Microsoft.Xna.Framework;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public interface IGraphicsPipelineAction : INamed, IDisposable
    {
        bool IsDisabled { get; set; }
        
        void Process(GameTime gameTime, IGraphicDeviceContext graphicDeviceContext, IReadOnlyList<IGraphicComponent> associatedComponents);
    }
}