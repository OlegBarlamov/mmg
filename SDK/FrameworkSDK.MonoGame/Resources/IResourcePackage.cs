using System;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Resources
{
    public interface IResourcePackage : IDisposable
    {
        bool IsLoaded { get; }
        void Load([NotNull] IContentLoaderApi content);

        void OnUnloaded();
    }
}