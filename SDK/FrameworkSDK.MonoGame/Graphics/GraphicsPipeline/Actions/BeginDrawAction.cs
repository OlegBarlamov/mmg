using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public class BeginDrawAction : GraphicsPipelineActionBase
    {
        private readonly BeginDrawConfig _beginDrawConfig;

        public BeginDrawAction([NotNull] string name, [NotNull] BeginDrawConfig beginDrawConfig) : base(name)
        {
            _beginDrawConfig = beginDrawConfig ?? throw new ArgumentNullException(nameof(beginDrawConfig));
        }

        public override void Process(GameTime gameTime, IGraphicDeviceContext graphicDeviceContext, IReadOnlyList<IGraphicComponent> associatedComponents)
        {
            graphicDeviceContext.BeginDraw(
                _beginDrawConfig.SortMode,
                _beginDrawConfig.BlendState,
                _beginDrawConfig.SamplerState,
                _beginDrawConfig.DepthStencilState,
                _beginDrawConfig.RasterizerState,
                _beginDrawConfig.Effect,
                _beginDrawConfig.TransformMatrix);
        }
    }
}