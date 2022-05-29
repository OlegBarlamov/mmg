using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.Basic
{
    public interface IRenderableMesh
    {
        Matrix World { get; }
        [NotNull] IRenderableComponent Parent { get; }
        [NotNull] IMeshGeometry Geometry { get; }
    }
}