using System;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    internal class FixedSystemPipelineAction : SystemPipelineAction
    {
        private Action<GameTime, IGraphicDeviceContext> Action { get; }

        public FixedSystemPipelineAction([NotNull] string name, [NotNull] IGraphicDeviceContext context, [NotNull] Action<GameTime, IGraphicDeviceContext> action)
            : base(name, context)
        {
            Action = action ?? throw new ArgumentNullException(nameof(action));
        }

        protected override void Process(GameTime gameTime, IGraphicDeviceContext context)
        {
            Action(gameTime, context);
        }
    }
}