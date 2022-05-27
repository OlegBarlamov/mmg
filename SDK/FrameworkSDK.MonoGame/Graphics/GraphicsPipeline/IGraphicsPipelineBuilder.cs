
using FrameworkSDK.MonoGame.Graphics.Services;
using FrameworkSDK.MonoGame.Services;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public interface IGraphicsPipelineBuilder
    {
        IRenderTargetsFactoryService RenderTargetsFactoryService { get; }

        //TODO temporary?
        GraphicsDevice GraphicsDevice { get; }
        
        IDisplayService DisplayService { get; }
        
        
        IGraphicsPipelineBuilder AddAction(IGraphicsPipelineAction action);

        IGraphicsPipeline Build();
    }
}