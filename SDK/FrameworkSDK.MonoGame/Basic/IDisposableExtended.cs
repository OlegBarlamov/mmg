using System;

namespace FrameworkSDK.MonoGame.Basic
{
    public interface IDisposableExtended : IDisposable
    {
        event EventHandler Disposed;
    }
}