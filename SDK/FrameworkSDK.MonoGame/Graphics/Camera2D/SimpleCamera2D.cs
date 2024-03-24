using Microsoft.Xna.Framework;
using MonoGameExtensions.Geometry;
using NetExtensions.Geometry;

namespace FrameworkSDK.MonoGame.Graphics.Camera2D
{
    public class SimpleCamera2D : IMutableCamera2D
    {
        public Vector2 Position { get; set; }
        public Vector2 DisplaySize { get; }
        
        public Vector2 Size { get; set; }

        public SimpleCamera2D(SizeInt displaySize) : this(displaySize, new Vector2(displaySize.Width, displaySize.Height))
        {
        }

        public SimpleCamera2D(SizeInt displaySize, Vector2 size)
        {
            DisplaySize = new Vector2(displaySize.Width, displaySize.Height);
            Size = size;
        }
        
        public Vector2 GetPosition()
        {
            return Position;
        }

        public Vector2 GetSize()
        {
            return Size;
        }

        public Rectangle ToDisplay(RectangleF worldRectangle)
        {
            return new Rectangle(ToDisplay(worldRectangle.Location).ToPoint(), (worldRectangle.Size / Size * DisplaySize).ToPoint());
        }

        public Vector2 ToDisplay(Vector2 worldPoint)
        {
            return (worldPoint - Position) / Size * DisplaySize;
        }

        public RectangleF FromDisplay(Rectangle displayRectangle)
        {
            return RectangleF.FromTopLeftAndSize(FromDisplay(displayRectangle.Location.ToVector2()), displayRectangle.Size.ToVector2() / DisplaySize * Size);
        }

        public Vector2 FromDisplay(Vector2 displayPoint)
        {
            return displayPoint / DisplaySize * Size + Position;
        }

        public void SetPosition(Vector2 position)
        {
            Position = position;
        }

        public void SetSize(Vector2 size)
        {
            Size = size;
        }
    }
}