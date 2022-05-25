using System;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Mvc
{
    public interface IScenesResolver : IDisposable
    {
        bool IsModelRegistered([NotNull] object model);

        [NotNull] SceneBase ResolveScene([NotNull] object model);
    }
}
