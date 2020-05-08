using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    internal sealed class FixedDrawPipelineAction : DrawPipelineAction
    {
        private Action<GameTime, IDrawContext> DrawAction { get; }

        public FixedDrawPipelineAction([NotNull] string name, [NotNull] IDrawContext drawContext, [NotNull] Action<GameTime, IDrawContext> drawAction)
            : base(name, drawContext)
        {
            if (drawContext == null) throw new ArgumentNullException(nameof(drawContext));
            DrawAction = drawAction ?? throw new ArgumentNullException(nameof(drawAction));
        }

        protected override void ProcessDraw(GameTime gameTime, IDrawContext drawContext, IEnumerable<IGraphicComponent> components)
        {
            DrawAction(gameTime, drawContext);
        }
    }
}