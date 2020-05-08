using FrameworkSDK.MonoGame.Graphics.RenderingTools;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public interface IGraphicsPipelineBuilder
    {
        IGraphicsPipeline Pipeline { get; }
        
        IDrawContext DrawContext { get; }
        
        IRenderContext RenderContext { get; }
        
        IGraphicDeviceContext GraphicDeviceContext { get; }

        IGraphicsPipelineBuilder AddAction(IGraphicsPipelineAction action);

        IGraphicsPipeline Build();
    }
}