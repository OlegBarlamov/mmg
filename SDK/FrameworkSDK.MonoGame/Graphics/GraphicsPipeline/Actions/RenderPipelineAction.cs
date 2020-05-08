using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public abstract class RenderPipelineAction : GraphicsPipelineAction
    {
        public sealed override string Name { get; }
        protected IRenderContext RenderContext { get; }

        protected RenderPipelineAction([NotNull] string name, [CanBeNull] IRenderContext renderContext = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            RenderContext = renderContext;
        }
        
        public override void Dispose()
        {
            RenderContext?.Dispose();
        }

        protected sealed override void Process(GameTime gameTime, IEnumerable<IGraphicComponent> components)
        {
            ProcessRender(gameTime, RenderContext, components);
        }

        protected abstract void ProcessRender(GameTime gameTime, IRenderContext renderContext, IEnumerable<IGraphicComponent> components);
    }
}