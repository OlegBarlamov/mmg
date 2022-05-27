using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline.Processing
{
    public interface IGraphicsPipelinePassAssociateService
    {
        [NotNull] IReadOnlyList<string> GetAssociatedPasses([NotNull] IGraphicComponent component);
    }
}