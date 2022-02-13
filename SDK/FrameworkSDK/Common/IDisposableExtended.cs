using System;

namespace FrameworkSDK.Common
{
    public interface IDisposableExtended : IDisposable
    {
        event EventHandler DisposedEvent;

        bool IsDisposed { get; }
    }
}
