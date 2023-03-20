using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.Camera2D
{
    public interface ICamera2D
    {
        Vector2 GetPosition();
        Vector2 GetSize();

        Vector2 ToDisplay(Vector2 worldPoint);
        Vector2 FromDisplay(Vector2 displayPoint);
    }
}