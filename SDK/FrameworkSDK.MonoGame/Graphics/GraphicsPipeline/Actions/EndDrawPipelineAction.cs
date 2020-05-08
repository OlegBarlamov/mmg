using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public class EndDrawPipelineAction : SystemPipelineAction
    {
        public EndDrawPipelineAction([NotNull] string name, [CanBeNull] IGraphicDeviceContext context = null)
            : base(name, context)
        {
        }

        protected override void Process(GameTime gameTime, IGraphicDeviceContext context)
        {
            context.EndDraw();
        }
    }
}