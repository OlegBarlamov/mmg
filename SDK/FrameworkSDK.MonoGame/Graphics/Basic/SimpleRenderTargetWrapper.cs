using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.Basic
{
    public class SimpleRenderTargetWrapper : IRenderTargetWrapper
    {
        public event EventHandler Disposed;
        
        public RenderTarget2D RenderTarget { get; }
        
        public SimpleRenderTargetWrapper([NotNull] RenderTarget2D renderTarget)
        {
            RenderTarget = renderTarget ?? throw new ArgumentNullException(nameof(renderTarget));
        }
        
        public void Dispose()
        {
            RenderTarget.Dispose();
            Disposed?.Invoke(this, EventArgs.Empty);
            Disposed = null;
        }
    }
}