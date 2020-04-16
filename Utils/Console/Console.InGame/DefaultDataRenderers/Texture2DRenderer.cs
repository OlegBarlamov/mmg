using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Console.InGame.DefaultDataRenderers
{
    public class Texture2DRenderer : CustomDataRenderer<Texture2D>
    {
        protected override Vector2 Measure(Texture2D data, Vector2 availableSize)
        {
            return new Vector2(
                Math.Min(availableSize.X, data.Width),
                Math.Min(availableSize.Y, data.Height));
        }

        protected override void Draw(Texture2D data, GameTime gameTime, SpriteBatch spriteBatch, Rectangle rectangle)
        {
            spriteBatch.Draw(data, rectangle, Color.White);
        }
    }
}