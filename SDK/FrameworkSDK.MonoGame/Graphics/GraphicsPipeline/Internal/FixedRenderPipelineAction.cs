using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    internal sealed class FixedRenderPipelineAction : RenderPipelineAction
    {
        private Action<GameTime, IRenderContext> RenderAction { get; }

        public FixedRenderPipelineAction([NotNull] string name, [NotNull] IRenderContext renderContext, [NotNull] Action<GameTime, IRenderContext> renderAction)
            : base(name, renderContext)
        {
            if (RenderAction == null) throw new ArgumentNullException(nameof(renderContext));
            RenderAction = renderAction ?? throw new ArgumentNullException(nameof(renderAction));
        }

        protected override void ProcessRender(GameTime gameTime, IRenderContext renderContext, IEnumerable<IGraphicComponent> components)
        {
            RenderAction(gameTime, renderContext);
        }
    }
}