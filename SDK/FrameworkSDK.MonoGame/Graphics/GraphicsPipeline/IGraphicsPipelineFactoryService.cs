using System;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public interface IGraphicsPipelineFactoryService
    {
        [NotNull] IGraphicsPipelineBuilder Create([CanBeNull] IGraphicsPipeline graphicsPipeline = null);
        
        [NotNull] IDrawContext CreateDrawContext();

        [NotNull] IGraphicDeviceContext CreateGraphicDeviceContext();

        [NotNull] IRenderContext CreateRenderContext();
    }
}