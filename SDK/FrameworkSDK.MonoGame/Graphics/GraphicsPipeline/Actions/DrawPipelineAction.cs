using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public abstract class DrawPipelineAction : GraphicsPipelineAction
    {
        public sealed override string Name { get; }
        protected IDrawContext DrawContext { get; }

        protected DrawPipelineAction([NotNull] string name, [CanBeNull] IDrawContext drawContext = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DrawContext = drawContext;
        }

        public override void Dispose()
        {
            DrawContext?.Dispose();
        }

        protected sealed override void Process(GameTime gameTime, IEnumerable<IGraphicComponent> components)
        {
            ProcessDraw(gameTime, DrawContext, components);
        }

        protected abstract void ProcessDraw(GameTime gameTime, IDrawContext drawContext, IEnumerable<IGraphicComponent> components);
    }
}