using System;
using FrameworkSDK.MonoGame.Graphics.Basic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameExtensions.Geometry;

namespace FrameworkSDK.MonoGame.Graphics.DrawableComponents.Stencils
{
    public class TextStencil : IDrawableStencil
    {
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                UpdateMeasurements();
            }
        }
        private SpriteFont SpriteFont { get; }
        public Color Color { get; set; }
        public float Rotation { get; set; }
        public Vector2 Origin { get; set; }
        public SpriteEffects SpriteEffect { get; set; }
        public float LayerDepth { get; set; }

        private string _text;
        private Vector2 _textSize;

        public TextStencil(
            [NotNull] String text,
            [NotNull] SpriteFont spriteFont,
            Color color, 
            float rotation = 0f,
            Vector2 origin = new Vector2(),
            SpriteEffects spriteEffect = SpriteEffects.None,
            float layerDepth = 0f
        )
        {
            _text = text ?? throw new ArgumentNullException(nameof(text));
            SpriteFont = spriteFont ?? throw new ArgumentNullException(nameof(spriteFont));
            Color = color;
            Rotation = rotation;
            
            UpdateMeasurements();
        }

        private void UpdateMeasurements()
        {
            _textSize = SpriteFont.MeasureString(Text);
        }
        
        public void DrawTo(GameTime gameTime, RectangleF screenRec, IDrawContext drawContext)
        {
            drawContext.DrawString(SpriteFont, _text, screenRec.Location, Color, Rotation, Origin, screenRec.Size / _textSize, SpriteEffect, LayerDepth);
        }
    }
}