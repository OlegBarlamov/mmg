using System;

namespace FrameworkSDK.MonoGame.Graphics.Pipeline
{
    public interface IGraphicsPipelineContext : IDisposable
    {
        IGraphicDevice GraphicDevice { get; }
        IRenderContext RenderContext { get; }
        IDrawContext DrawContext { get; }
    }
}
