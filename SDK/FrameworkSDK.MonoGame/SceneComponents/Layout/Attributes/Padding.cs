namespace FrameworkSDK.MonoGame.SceneComponents.Layout.Attributes
{
    public struct Padding
    {
        public float Left;
        public float Right;
        public float Top;
        public float Bottom;

        public Padding(float left, float right, float top, float bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }
    }

    public static class PaddingExtensions
    {
        public static Padding Inverse(this Padding padding)
        {
            return new Padding(-padding.Left, -padding.Right, -padding.Top, -padding.Bottom);
        }
    }
}