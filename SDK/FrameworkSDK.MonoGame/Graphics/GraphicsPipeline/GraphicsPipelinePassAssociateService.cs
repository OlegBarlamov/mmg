using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline.Processing
{
    [UsedImplicitly]
    internal class GraphicsPipelinePassAssociateService : IGraphicsPipelinePassAssociateService
    {
        public IReadOnlyList<string> GetAssociatedPasses(IGraphicComponent component)
        {
            return component.GraphicsPassNames;
        }
    }
}