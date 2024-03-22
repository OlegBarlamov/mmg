using Microsoft.Xna.Framework;

namespace MonoGameExtensions.Geometry
{
    public static class RectanglesExtensions
    {
        public static Rectangle ToRectangle(this RectangleF rectangle)
        {
            return new Rectangle((int) rectangle.X, (int) rectangle.Y, (int) rectangle.Width, (int) rectangle.Height);
        }
        
        public static RectangleF ToRectangleF(this Rectangle rectangle)
        {
            return new RectangleF(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }
    }
}