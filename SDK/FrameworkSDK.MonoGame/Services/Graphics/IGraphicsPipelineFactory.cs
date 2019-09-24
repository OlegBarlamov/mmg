using FrameworkSDK.MonoGame.Graphics;

namespace FrameworkSDK.MonoGame.Services.Graphics
{
    public interface IGraphicsPipelineFactory
    {
        IGraphicsPipeline CreateDefaultPipeline();
    }
}
