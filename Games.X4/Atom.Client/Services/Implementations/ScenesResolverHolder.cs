using System;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;

namespace Atom.Client.Services.Implementations
{
    public class ScenesResolverHolder : IDisposable
    {
        [NotNull] public IScenesResolver ScenesResolver { get; internal set; }
        
        public void Dispose()
        {
            if (ScenesResolver != null)
            {
                ScenesResolver.Dispose();
            }
        }
    }
}