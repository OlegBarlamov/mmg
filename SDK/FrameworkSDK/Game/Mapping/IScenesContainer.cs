using System;
using JetBrains.Annotations;

namespace FrameworkSDK.Game.Mapping
{
    public interface IScenesContainer : IScenesRegistrator, IDisposable
    {
        [NotNull] IScenesResolver CreateResolver();
    }
}
