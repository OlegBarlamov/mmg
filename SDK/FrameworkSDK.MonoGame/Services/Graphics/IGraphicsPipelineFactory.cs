using FrameworkSDK.MonoGame.Graphics;
using FrameworkSDK.MonoGame.Graphics.Pipeline;

namespace FrameworkSDK.MonoGame.Services.Graphics
{
    public interface IGraphicsPipelineFactory
    {
        IGraphicsPipeline CreateDefaultPipeline();
    }
}
