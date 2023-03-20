using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using JetBrains.Annotations;
using NetExtensions.Collections;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    internal interface IGraphicsPipelineFactoryService
    {
        [NotNull] IGraphicsPipelineBuilder Create(IReadOnlyObservableList<IGraphicComponent> graphicComponents);
        
        [NotNull] IGraphicDeviceContext CreateGraphicDeviceContext();
    }
}