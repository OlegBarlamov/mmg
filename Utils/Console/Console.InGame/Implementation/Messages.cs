using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Console.InGame.Implementation
{
    internal interface IRenderingMessage
    {
        Vector2 Size { get; }

        void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, Vector2 availableSize);
    }
    
    internal class TextMessage : IRenderingMessage
    {
        public Vector2 Size { get; }
        
        private SpriteFont SpriteFont { get; }
        private string Text { get; }
        private Color Color { get; }

        public TextMessage([NotNull] string text, Color color, Vector2 size, [NotNull] SpriteFont spriteFont)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Color = color;
            Size = size;
            SpriteFont = spriteFont ?? throw new ArgumentNullException(nameof(spriteFont));
        }
        
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, Vector2 availableSize)
        {
            spriteBatch.DrawString(SpriteFont, Text, position, Color);
        }
    }

    internal class CustomMessage : IRenderingMessage
    {
        public Vector2 Size { get; private set; }
        
        private object Data { get; }
        private IDataRenderer Renderer { get; }

        public CustomMessage([NotNull] object data, [NotNull] IDataRenderer renderer, Vector2 initialSize)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            Renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
            Size = initialSize;
        }
        
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, Vector2 availableSize)
        {
            Size = Renderer.Measure(Data, availableSize);
            Renderer.Draw(Data, gameTime, spriteBatch, new Rectangle(position.ToPoint(), Size.ToPoint()));
        }
    }
}