using System.Collections.Generic;

namespace FrameworkSDK.MonoGame.Graphics.Basic
{
    public interface IGraphicComponent : IDrawableComponent, IRenderableComponent
    {
        IReadOnlyList<string> GraphicsPassNames { get; }
    }
}
