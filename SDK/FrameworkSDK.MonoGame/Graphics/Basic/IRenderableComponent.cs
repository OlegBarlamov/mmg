using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Meshes;

namespace FrameworkSDK.MonoGame.Graphics.Basic
{
    public interface IRenderableComponent
    {
        IReadOnlyDictionary<string, IReadOnlyList<IRenderableMesh>> MeshesByPass { get; }
    }
}
