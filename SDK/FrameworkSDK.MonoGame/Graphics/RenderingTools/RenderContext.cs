using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.RenderingTools
{
    public class RenderContext : IRenderContext
    {
        private GraphicsDevice GraphicsDevice { get; }

        public RenderContext([NotNull] GraphicsDevice graphicsDevice)
        {
            
            
        }
        
        public void Dispose()
        {
            
        }
    }
}
