using System;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public class BeginDrawPipelineAction : SystemPipelineAction
    {
        private BeginDrawConfig BeginDrawConfig { get; }

        public BeginDrawPipelineAction([NotNull] string name, [NotNull] BeginDrawConfig beginDrawConfig, [CanBeNull] IGraphicDeviceContext context = null)
            : base(name, context)
        {
            BeginDrawConfig = beginDrawConfig ?? throw new ArgumentNullException(nameof(beginDrawConfig));
        }

        protected override void Process(GameTime gameTime, IGraphicDeviceContext context)
        {
            context.BeginDraw(
                BeginDrawConfig.SortMode,
                BeginDrawConfig.BlendState,
                BeginDrawConfig.SamplerState,
                BeginDrawConfig.DepthStencilState,
                BeginDrawConfig.RasterizerState,
                BeginDrawConfig.Effect,
                BeginDrawConfig.TransformMatrix);
        }
    }
}