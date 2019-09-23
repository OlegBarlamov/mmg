using FrameworkSDK.Game.Graphics;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.Game
{
    public interface IRenderableComponent
    {
        void Render(GameTime gameTime, IRenderContext context);
    }
}
