using Microsoft.Xna.Framework;
using MonoGameExtensions.Geometry;

namespace FrameworkSDK.MonoGame.Graphics.Camera2D
{
    public interface ICamera2D
    {
        Vector2 GetPosition();
        Vector2 GetSize();

        Rectangle ToDisplay(RectangleF worldRectangle);
        Vector2 ToDisplay(Vector2 worldPoint);
        RectangleF FromDisplay(Rectangle displayRectangle);
        Vector2 FromDisplay(Vector2 displayPoint);
    }
}