using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public abstract class SystemPipelineAction : GraphicsPipelineAction
    {
        public sealed override string Name { get; }
        protected IGraphicDeviceContext GraphicsContext { get; }

        protected SystemPipelineAction([NotNull] string name, [CanBeNull] IGraphicDeviceContext context = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            GraphicsContext = context;
        }
        
        public override void Dispose()
        {
            GraphicsContext?.Dispose();
        }

        protected sealed override void Process(GameTime gameTime, IEnumerable<IGraphicComponent> components)
        {
            Process(gameTime, GraphicsContext);
        }

        protected abstract void Process(GameTime gameTime, IGraphicDeviceContext context);
    }
}