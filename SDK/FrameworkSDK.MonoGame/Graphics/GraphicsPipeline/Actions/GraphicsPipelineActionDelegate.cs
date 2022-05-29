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

        protected List<IGraphicComponent> AttachedComponents { get; } = new List<IGraphicComponent>();
        
        [NotNull] private readonly GraphicsActionDelegate _processAction;
        
        public void Process(GameTime gameTime, IGraphicDeviceContext graphicDeviceContext)
        {
            _processAction.Invoke(gameTime, graphicDeviceContext, AttachedComponents);
        }

        public void OnComponentAttached(IGraphicComponent attachingComponent)
        {
            AttachedComponents.Add(attachingComponent);
        }

        public void OnComponentDetached(IGraphicComponent detachingComponent)
        {
            AttachedComponents.Remove(detachingComponent);
        }

        public GraphicsPipelineActionDelegate([NotNull] string name, [NotNull] GraphicsActionDelegate processAction)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException(nameof(name));
            Name = name;
            _processAction = processAction ?? throw new ArgumentNullException(nameof(processAction));
        }

        public void Dispose()
        {
            AttachedComponents.Clear();
        }
    }
}