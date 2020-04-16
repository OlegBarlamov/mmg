using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Console.InGame
{
    public abstract class CustomDataRenderer<TData> : IDataRenderer
    {
        protected abstract Vector2 Measure(TData data, Vector2 availableSize);
        protected abstract void Draw(TData data, GameTime gameTime, SpriteBatch spriteBatch, Rectangle rectangle);
        Vector2 IDataRenderer.Measure(object data, Vector2 availableSize)
        {
            return Measure((TData) data, availableSize);
        }

        void IDataRenderer.Draw(object data, GameTime gameTime, SpriteBatch spriteBatch, Rectangle rectangle)
        {
            Draw((TData)data, gameTime, spriteBatch, rectangle);
        }
    }

    public static class RegisterDataRendererHelper
    {
        public static void RegisterDataRenderer<TData>(this InGameConsoleController controller, CustomDataRenderer<TData> renderer)
        {
            controller.RegisterDataRenderer<TData>(renderer);
        }
    }
}