using FrameworkSDK.MonoGame.Graphics.Services;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public interface IGraphicsPipelineBuilder
    {
        IGraphicsPipelineBuilder AddAction(IGraphicsPipelineAction action);

        IGraphicsPipeline Build();
    }
}