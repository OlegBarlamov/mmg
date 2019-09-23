using FrameworkSDK.Game.Graphics;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.Game
{
    public interface IDrawableComponent
    {
        void Draw(GameTime gameTime, IDrawContext context);
    }
}
