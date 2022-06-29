using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.Materials;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.Meshes
{
    public interface IRenderableMesh
    {
        Matrix World { get; }
        [NotNull] IRenderableComponent Parent { get; }
        [NotNull] IMeshGeometry Geometry { get; }
        [NotNull] IMeshMaterial Material { get; }
    }
}