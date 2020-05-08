using System;
using FrameworkSDK.MonoGame.Graphics.Basic;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline.Processing
{
    public interface IGraphicsPipelinePassAssociateService : IDisposable
    {
        void Initialize();
        [CanBeNull] string GetAssociatedPass([NotNull] IGraphicComponent component);
    }
}