using System;
using FrameworkSDK.MonoGame.Graphics.Basic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameExtensions.Geometry;

namespace FrameworkSDK.MonoGame.Graphics.DrawableComponents.Stencils
{
    public class TextureStencil : IDrawableStencil
    {
        public Texture2D Texture2D { get; }
        public Color Color { get; set; }
        public Rectangle? SourceRec { get; set; }
        public float Rotation { get; set; }
        public Vector2 Origin { get; set; }
        public SpriteEffects SpriteEffect { get; set; }
        public float LayerDepth { get; set; }

        public TextureStencil(
            [NotNull] Texture2D texture2D,
            Color color,
            Rectangle? sourceRec = null,
            float rotation = 0f,
            Vector2 origin = new Vector2(),
            SpriteEffects spriteEffect = SpriteEffects.None,
            float layerDepth = 0f
            )
        {
            Texture2D = texture2D ?? throw new ArgumentNullException(nameof(texture2D));
            Color = color;
            SourceRec = sourceRec;
            Rotation = rotation;
            Origin = origin;
            SpriteEffect = spriteEffect;
            LayerDepth = layerDepth;
        }
        
        public void DrawTo(GameTime gameTime, RectangleF screenRec, IDrawContext drawContext)
        {
            drawContext.Draw(Texture2D, screenRec, SourceRec, Color, Rotation, Origin, SpriteEffect, LayerDepth);
        }
    }
}