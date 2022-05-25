using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public class DrawComponentsAction : IGraphicsPipelineAction
    {
        public string Name { get; }
        public bool IsDisabled { get; set; }

        public DrawComponentsAction([NotNull] string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
        
        public void Process(GameTime gameTime, IGraphicDeviceContext graphicDeviceContext, IReadOnlyList<IGraphicComponent> associatedComponents)
        {
            for (int i = 0; i < associatedComponents.Count; i++)
            {
                var component = associatedComponents[i];
                component.Draw(gameTime, graphicDeviceContext);
            }
        }
    }
}