using Microsoft.Xna.Framework;
using MonoGameExtensions.Geometry;

namespace FrameworkSDK.MonoGame.Graphics.Camera2D
{
    public static class CamerasExtensions
    {
        public static void CenterTo(this IMutableCamera2D camera, Vector2 position)
        {
            camera.SetPosition(position - camera.GetSize() / 2);
        }
        
        public static bool IsRectangleVisible(this ICamera2D camera, RectangleF rectangle)
        {
            var pos = camera.GetPosition();
            var size = camera.GetSize();
            var end = pos + size;
            return rectangle.Right > pos.X &&
                   rectangle.Left < end.X &&
                   rectangle.Bottom > pos.Y &&
                   rectangle.Top < end.Y;
        }
    }
}