using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.Camera2D
{
    public static class CamerasExtensions
    {
        public static void CenterTo(this IMutableCamera2D camera, Vector2 position)
        {
            camera.SetPosition(position - camera.GetSize() / 2);
        }
    }
}