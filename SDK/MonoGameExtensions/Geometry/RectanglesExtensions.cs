using System;
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
        
        public static RectangleF GetMinimalContainingRectangle(this RectangleF rect1, RectangleF rect2)
        {
            // Calculate the minimum X and Y
            float minX = Math.Min(rect1.Left, rect2.Left);
            float minY = Math.Min(rect1.Top, rect2.Top);
        
            // Calculate the maximum right and bottom
            float maxX = Math.Max(rect1.Right, rect2.Right);
            float maxY = Math.Max(rect1.Bottom, rect2.Bottom);
        
            // Calculate width and height of the minimal containing rectangle
            float width = maxX - minX;
            float height = maxY - minY;
        
            // Return the minimal containing rectangle
            return new RectangleF(minX, minY, width, height);
        }
    }
}