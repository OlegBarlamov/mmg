using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.Pipelines;
using Microsoft.Xna.Framework;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public abstract class GraphicsPipelineAction : IGraphicsPipelineAction
    {
        public abstract string Name { get; }
        public virtual bool IsCritical { get; } = false;

        public bool IsDisabled { get; set; } = false;
        
        protected IPipelineContext Context { get; private set; }

        public abstract void Dispose();
        
        void IGraphicsPipelineAction.Process(IPipelineContext context, GameTime gameTime, IEnumerable<IGraphicComponent> components)
        {
            Context = context;
            if (!IsDisabled)
            {
                Process(gameTime, components);
            }
        }

        protected abstract void Process(GameTime gameTime, IEnumerable<IGraphicComponent> components);
        
        void IPipelineAction.Process(IPipelineContext pipelineContext)
        {
            throw new NotImplementedException();
        }
    }
}