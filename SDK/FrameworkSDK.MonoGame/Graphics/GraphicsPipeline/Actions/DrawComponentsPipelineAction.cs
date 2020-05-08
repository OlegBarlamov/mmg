using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public class DrawComponentsPipelineAction : DrawPipelineAction
    {
        public DrawComponentsPipelineAction([NotNull] string name, [CanBeNull] IDrawContext drawContext = null)
            : base(name, drawContext)
        {
        }

        protected override void ProcessDraw(GameTime gameTime, IDrawContext drawContext, IEnumerable<IGraphicComponent> components)
        {
            foreach (var component in components)
            {
                component.Draw(gameTime, drawContext);
            }
        }
    }
}