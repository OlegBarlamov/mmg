using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.Camera2D
{
    public static class Camera2DExtensions
    {
        public static Rectangle Viewport(this ICamera2D camera)
        {
            return new Rectangle((int)camera.GetPosition().X, (int)camera.GetPosition().Y, (int)camera.GetSize().X, (int)camera.GetSize().Y);
        }

        public static Vector2? ToDisplay(this ICamera2D camera, Vector2? point)
        {
            if (!point.HasValue)
            {
                return null;
            }

            return camera.ToDisplay(point.Value);
        }
        
        public static Point ToDisplay(this ICamera2D camera, Point point)
        {
            var displayPoint = camera.ToDisplay(new Vector2(point.X, point.Y));
            return new Point((int) displayPoint.X, (int) displayPoint.Y);
        }
        
        public static Rectangle ToDisplay(this ICamera2D camera, Rectangle rectangle)
        {
            return new Rectangle(
                camera.ToDisplay(new Point(rectangle.Left, rectangle.Top)),
                camera.ToDisplay(new Point(rectangle.Width, rectangle.Height))
                );
        }

        public static Rectangle? ToDisplay(this ICamera2D camera, Rectangle? rectangle)
        {
            if (!rectangle.HasValue)
            {
                return null;
            }

            return camera.ToDisplay(rectangle.Value);
        }
    }
}