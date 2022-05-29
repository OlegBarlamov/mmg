using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public abstract class GraphicsPipelineActionBase : IGraphicsPipelineAction
    {
        public string Name { get; }

        public bool IsDisabled { get; set; }
        
        protected List<IGraphicComponent> AttachedComponents { get; } = new List<IGraphicComponent>();
        
        protected GraphicsPipelineActionBase([NotNull] string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
        
        public void Process(GameTime gameTime, IGraphicDeviceContext graphicDeviceContext)
        {
            Process(gameTime, graphicDeviceContext, AttachedComponents);
        }

        public virtual void OnComponentAttached(IGraphicComponent attachingComponent)
        {
            AttachedComponents.Add(attachingComponent);
        }

        public virtual void OnComponentDetached(IGraphicComponent detachingComponent)
        {
            AttachedComponents.Remove(detachingComponent);
        }

        void IDisposable.Dispose()
        {
            Dispose();
            AttachedComponents.Clear();
        }

        public abstract void Process(GameTime gameTime, IGraphicDeviceContext graphicDeviceContext,
            IReadOnlyList<IGraphicComponent> associatedComponents);

        protected virtual void Dispose()
        {
            
        }
    }
}