using FrameworkSDK.Game.Graphics;

namespace FrameworkSDK.Services.Graphics
{
    public interface IGraphicsPipelineFactory
    {
        IGraphicsPipeline CreateDefaultPipeline();
    }
}
