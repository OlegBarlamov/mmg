using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public class GraphicsPipelineActionDelegate : IGraphicsPipelineAction
    {
        public string Name { get; }
        public bool IsDisabled { get; set; }

        [NotNull] private readonly GraphicsActionDelegate _processAction;

        public GraphicsPipelineActionDelegate([NotNull] string name, [NotNull] GraphicsActionDelegate processAction)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException(nameof(name));
            Name = name;
            _processAction = processAction ?? throw new ArgumentNullException(nameof(processAction));
        }
        public void Process(GameTime gameTime, IGraphicDeviceContext graphicDeviceContext, IReadOnlyList<IGraphicComponent> associatedComponents)
        {
            _processAction.Invoke(gameTime, graphicDeviceContext, associatedComponents);
        }
    }
}