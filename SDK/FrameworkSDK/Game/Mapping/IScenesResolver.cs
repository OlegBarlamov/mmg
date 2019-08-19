using System;
using FrameworkSDK.Game.Scenes;
using JetBrains.Annotations;

namespace FrameworkSDK.Game.Mapping
{
    public interface IScenesResolver : IDisposable
    {
        bool IsModelRegistered([NotNull] object model);

        [NotNull] Scene ResolveScene([NotNull] object model);
    }
}
