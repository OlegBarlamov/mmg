using System;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.Basic
{
    public class DrawParameters
    {
        public Vector2? Position { get; set; } = null;
        public Rectangle? DestinationRectangle { get; set; } = null;
        public Rectangle? SourceRectangle { get; set; } = null;
        public Vector2? Origin { get; set; } = null;
        public float Rotation { get; set; } = 0.0f;
        public Vector2? Scale { get; set; } = null;
        public Color? Color { get; set; } = null;
        public SpriteEffects Effects { get; set; } = SpriteEffects.None;
        public float LayerDepth { get; set; } = 0.0f;

        public static DrawParameters StretchToFullScreen([NotNull] IDisplayService displayService)
        {
            if (displayService == null) throw new ArgumentNullException(nameof(displayService));
            
            return new DrawParameters
            {
                DestinationRectangle = new Rectangle(0, 0, displayService.PreferredBackBufferWidth, displayService.PreferredBackBufferHeight)
            };
        }
    }
}