using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public class ClearPipelineAction : SystemPipelineAction
    {
        private Color ClearColor { get; }

        public ClearPipelineAction([NotNull] string name, Color clearColor, [CanBeNull] IGraphicDeviceContext context = null) 
            : base(name, context)
        {
            ClearColor = clearColor;
        }

        protected override void Process(GameTime gameTime, IGraphicDeviceContext context)
        {
            context.Clear(ClearColor);
        }
    }
}