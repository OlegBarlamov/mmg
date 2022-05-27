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

        protected GraphicsPipelineActionBase([NotNull] string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
        
        void IDisposable.Dispose()
        {
            Dispose();
        }

        public abstract void Process(GameTime gameTime, IGraphicDeviceContext graphicDeviceContext,
            IReadOnlyList<IGraphicComponent> associatedComponents);

        protected virtual void Dispose()
        {
            
        }
    }
}