using Microsoft.Xna.Framework;
using MonoGameExtensions.Geometry;

namespace FrameworkSDK.MonoGame.Graphics.Camera2D
{
    public static class Camera2DExtensions
    {
        public static Rectangle Viewport(this ICamera2D camera)
        {
            var position = camera.GetPosition();
            var size = camera.GetSize();
            return new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
        }

        public static void SetViewport(this IMutableCamera2D camera, Rectangle viewport)
        {
            camera.SetPosition(new Vector2(viewport.X, viewport.Y));
            camera.SetSize(new Vector2(viewport.Width, viewport.Height));
        }

        public static Vector2? ToDisplay(this ICamera2D camera, Vector2? worldPoint)
        {
            return worldPoint.HasValue ? (Vector2?) camera.ToDisplay(worldPoint.Value) : null;
        }
        
        public static Rectangle? ToDisplay(this ICamera2D camera, RectangleF? worldRectangle)
        {
            return worldRectangle.HasValue ? (Rectangle?) camera.ToDisplay(worldRectangle.Value) : null;
        }
    }
}