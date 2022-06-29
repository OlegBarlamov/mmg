using FrameworkSDK.MonoGame.Basic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.Materials;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Graphics.Meshes
{
    public interface IRenderableMesh : IScenePlaceable
    {
        [NotNull] IRenderableComponent Parent { get; set; }
        [NotNull] IMeshGeometry Geometry { get; }
        [NotNull] IMeshMaterial Material { get; set; }
    }
}