using System.Collections.Generic;

namespace FrameworkSDK.MonoGame.Graphics.Basic
{
    public interface IRenderableComponent
    {
        IReadOnlyDictionary<string, IReadOnlyList<IRenderableMesh>> MeshesByPass { get; }
    }
}
