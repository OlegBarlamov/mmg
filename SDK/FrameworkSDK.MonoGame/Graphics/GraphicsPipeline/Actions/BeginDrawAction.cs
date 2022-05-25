using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public class BeginDrawAction : IGraphicsPipelineAction
    {
        public string Name { get; }
        public bool IsDisabled { get; set; }
        
        private readonly BeginDrawConfig _beginDrawConfig;

        public BeginDrawAction([NotNull] string name, [NotNull] BeginDrawConfig beginDrawConfig)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _beginDrawConfig = beginDrawConfig ?? throw new ArgumentNullException(nameof(beginDrawConfig));
        }

        public void Process(GameTime gameTime, IGraphicDeviceContext graphicDeviceContext, IReadOnlyList<IGraphicComponent> associatedComponents)
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