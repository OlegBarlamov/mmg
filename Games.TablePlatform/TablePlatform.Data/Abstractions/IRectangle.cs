using Microsoft.Xna.Framework;

namespace TablePlatform.Data
{
    public interface IRectangle : ISizable
    {
        float Left { get; }
        float Right { get; }
        float Top { get; }
        float Bottom { get; }
        
        Point LeftTop { get; }
    }
}