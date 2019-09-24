using FrameworkSDK.MonoGame.Graphics;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.GameStructure
{
    public interface IRenderableComponent
    {
        void Render(GameTime gameTime, IRenderContext context);
    }
}
