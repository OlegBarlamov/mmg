using FrameworkSDK.Common;
using Microsoft.Xna.Framework.Graphics;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Graphics
{
    public interface IRenderTargetWrapper : IDisposableExtended
    {
        RenderTarget2D RenderTarget { get; }
    }
}