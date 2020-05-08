using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public class RenderComponentsPipelineAction : RenderPipelineAction
    {
        public RenderComponentsPipelineAction([NotNull] string name, [CanBeNull] IRenderContext renderContext = null)
            : base(name, renderContext)
        {
        }

        protected override void ProcessRender(GameTime gameTime, IRenderContext renderContext, IEnumerable<IGraphicComponent> components)
        {
            foreach (var component in components)
            {
                component.Render(gameTime, renderContext);
            }
        }
    }
}