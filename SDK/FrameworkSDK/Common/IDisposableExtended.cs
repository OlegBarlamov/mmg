using System;

namespace FrameworkSDK.Common
{
    internal interface IDisposableExtended : IDisposable
    {
        event Action DisposedEvent;

        bool IsDisposed { get; }
    }
}
