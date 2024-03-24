using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Resources
{
    public interface ITexturePrimitivesGenerator
    {
        Texture2D Circle(int radius, Color fillColor, Color? strokeColor = null, int? strokeThickness = null, Color? outerColor = null);
    }
}