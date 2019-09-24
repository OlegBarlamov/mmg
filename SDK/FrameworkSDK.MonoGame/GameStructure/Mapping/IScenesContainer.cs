using System;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.GameStructure.Mapping
{
    public interface IScenesContainer : IScenesRegistrator, IDisposable
    {
        [NotNull] IScenesResolver CreateResolver();
    }
}
