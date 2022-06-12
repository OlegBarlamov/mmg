using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.Basic
{
    public interface IGraphicComponent : IDrawableComponent, IRenderableComponent, INamed
    {
        IReadOnlyList<string> GraphicsPassNames { get; }
        BoundingBox? BoundingBox { get; }
    }
}
