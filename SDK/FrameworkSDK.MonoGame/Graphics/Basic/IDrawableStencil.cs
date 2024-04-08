using Microsoft.Xna.Framework;
using MonoGameExtensions.Geometry;

namespace FrameworkSDK.MonoGame.Graphics.Basic
{
    public interface IDrawableStencil
    {
        void DrawTo(GameTime gameTime, RectangleF screenRec, IDrawContext drawContext);
    }
}