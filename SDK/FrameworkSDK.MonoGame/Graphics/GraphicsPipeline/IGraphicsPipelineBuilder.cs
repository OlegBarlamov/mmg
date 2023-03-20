using System;
using FrameworkSDK.MonoGame.Graphics.Services;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public interface IGraphicsPipelineBuilder
    {
        IRenderTargetsFactoryService RenderTargetsFactoryService { get; }
        IVideoBuffersFactoryService VideoBuffersFactoryService { get; }

        IGraphicsPipelineBuilder AddAction(IGraphicsPipelineAction action);
        IGraphicsPipelineBuilder InsertAction(string actionName, IGraphicsPipelineAction action);
        IGraphicsPipelineBuilder RemoveAction(string actionName);

        IGraphicsPipeline Build(IDisposable resources = null);
    }
}