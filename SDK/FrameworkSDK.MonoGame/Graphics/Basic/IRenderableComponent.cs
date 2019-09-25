using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.Basic
{
    public interface IRenderableComponent
    {
        void Render(GameTime gameTime, IRenderContext context);
    }
}
