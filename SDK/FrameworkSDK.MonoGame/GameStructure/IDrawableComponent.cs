using FrameworkSDK.MonoGame.Graphics;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.GameStructure
{
    public interface IDrawableComponent
    {
        void Draw(GameTime gameTime, IDrawContext context);
    }
}
