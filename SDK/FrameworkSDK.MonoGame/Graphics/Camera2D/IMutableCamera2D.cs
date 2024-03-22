using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.Camera2D
{
    public interface IMutableCamera2D : ICamera2D
    {
        void SetPosition(Vector2 position);
        void SetSize(Vector2 size);
    }
}