using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.Pipelines;
using Microsoft.Xna.Framework;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public interface IGraphicsPipelineAction : IPipelineAction, IDisposable
    {
        bool IsDisabled { get; set; }
        
        void Process(IPipelineContext context, GameTime gameTime, IEnumerable<IGraphicComponent> components);
    }
}