using System;
using FrameworkSDK.MonoGame.Graphics.Services;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public abstract class GraphicPipelineBuilderWrapper : IGraphicsPipelineBuilder
    {
        protected IGraphicsPipelineBuilder Builder { get; }
        
        IRenderTargetsFactoryService IGraphicsPipelineBuilder.RenderTargetsFactoryService => Builder.RenderTargetsFactoryService;
        IVideoBuffersFactoryService IGraphicsPipelineBuilder.VideoBuffersFactoryService => Builder.VideoBuffersFactoryService;

        protected GraphicPipelineBuilderWrapper([NotNull] IGraphicsPipelineBuilder builder)
        {
            Builder = builder ?? throw new ArgumentNullException(nameof(builder));
        }

        IGraphicsPipelineBuilder IGraphicsPipelineBuilder.AddAction(IGraphicsPipelineAction action)
        {
            Builder.AddAction(action);
            return this;
        }

        public IGraphicsPipelineBuilder InsertAction(string actionName, IGraphicsPipelineAction action)
        {
            Builder.InsertAction(actionName, action);
            return this;
        }

        public IGraphicsPipelineBuilder RemoveAction(string actionName)
        {
            Builder.RemoveAction(actionName);
            return this;
        }

        public virtual IGraphicsPipeline Build(IDisposable resources = null)
        {
            return Builder.Build(resources);
        }
    }
}