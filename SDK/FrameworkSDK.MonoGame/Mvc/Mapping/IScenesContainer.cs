using System;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Mvc
{
    public interface IScenesContainer : IScenesRegistrator, IDisposable
    {
        [NotNull] IScenesResolver CreateResolver();
    }
}
