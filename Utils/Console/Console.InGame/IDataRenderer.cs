using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Console.InGame
{
    public interface IDataRenderer
    {
        Vector2 Measure(object data, Vector2 availableSize);
        void Draw(object data, GameTime gameTime, SpriteBatch spriteBatch, Rectangle rectangle);
    }
}