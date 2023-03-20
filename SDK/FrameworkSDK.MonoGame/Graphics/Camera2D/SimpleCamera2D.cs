using Microsoft.Xna.Framework;
using NetExtensions.Geometry;

namespace FrameworkSDK.MonoGame.Graphics.Camera2D
{
    public class SimpleCamera2D : ICamera2D
    {
        public Vector2 Position { get; set; }
        public SizeInt DisplaySize { get; }
        
        public Vector2 Size { get; set; }

        public SimpleCamera2D(SizeInt displaySize)
        {
            DisplaySize = displaySize;
            Size = new Vector2(displaySize.Width, displaySize.Height);
        }
        
        public Vector2 GetPosition()
        {
            return Position;
        }

        public Vector2 GetSize()
        {
            return Size;
        }

        public Vector2 ToDisplay(Vector2 worldPoint)
        {
            return worldPoint - Position;
        }

        public Vector2 FromDisplay(Vector2 displayPoint)
        {
            return displayPoint + Position;
        }
    }
}