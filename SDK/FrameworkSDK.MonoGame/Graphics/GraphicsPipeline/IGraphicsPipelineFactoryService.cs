using System;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using JetBrains.Annotations;
using NetExtensions.Collections;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public interface IGraphicsPipelineFactoryService
    {
        [NotNull] IGraphicsPipelineBuilder Create(IReadOnlyObservableList<IGraphicComponent> graphicComponents);
        
        [NotNull] IDrawContext CreateDrawContext();

        [NotNull] IGraphicDeviceContext CreateGraphicDeviceContext();

        [NotNull] IRenderContext CreateRenderContext();
    }
}