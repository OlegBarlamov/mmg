using System;
using FrameworkSDK.MonoGame.GameStructure.Scenes;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.GameStructure.Mapping
{
    public interface IScenesResolver : IDisposable
    {
        bool IsModelRegistered([NotNull] object model);

        [NotNull] Scene ResolveScene([NotNull] object model);
    }
}
