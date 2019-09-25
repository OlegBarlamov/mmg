using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.Basic
{
    public interface IDrawableComponent
    {
        void Draw(GameTime gameTime, IDrawContext context);
    }
}
